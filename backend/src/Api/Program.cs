using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using AutoMarket.Application;
using AutoMarket.Domain;
using AutoMarket.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 65536);
var connection = builder.Configuration.GetConnectionString("SupabasePostgres")
    ?? throw new InvalidOperationException("Configure ConnectionStrings__SupabasePostgres in backend environment variables.");
var key = builder.Configuration["Jwt:Key"] ?? "";
if (Encoding.UTF8.GetByteCount(key) < 32) throw new InvalidOperationException("Configure Jwt__Key with at least 32 bytes of cryptographically random material.");
var jwt = new JwtSettings(key, builder.Configuration["Jwt:Issuer"] ?? "AutoMarket", builder.Configuration["Jwt:Audience"] ?? "AutoMarket.Web");
builder.Services.AddSingleton(jwt);
builder.Services.AddDbContext<MarketplaceDb>(o => o.UseNpgsql(connection, n => { n.CommandTimeout(10); n.MigrationsHistoryTable("__EFMigrationsHistory", "automarket"); }));
builder.Services.AddScoped<IStore>(p => p.GetRequiredService<MarketplaceDb>());
builder.Services.AddScoped<IIdentity, Identity>();
builder.Services.AddScoped<ICatalogSearch, PostgresSearch>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<WorkflowService>();
var aiBaseUrl = builder.Configuration["AiService:BaseUrl"];
var aiSettings = new AiSettings(builder.Configuration["AiService:ApiKey"] ?? "");
builder.Services.AddSingleton(aiSettings);
builder.Services.AddHttpClient<IAiService, AiClient>(client =>
{
    if (Uri.TryCreate(aiBaseUrl, UriKind.Absolute, out var address)) client.BaseAddress = address;
    client.Timeout = TimeSpan.FromSeconds(4);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new() { ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true,
        ValidateIssuerSigningKey = true, ValidIssuer = jwt.Issuer, ValidAudience = jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)), ClockSkew = TimeSpan.FromSeconds(15) };
    o.Events = new JwtBearerEvents { OnTokenValidated = async context =>
    {
        if (!Guid.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)) { context.Fail("Invalid account."); return; }
        var db = context.HttpContext.RequestServices.GetRequiredService<MarketplaceDb>();
        var user = await db.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Id == id && u.IsActive, context.HttpContext.RequestAborted);
        if (user is null || user.Role != context.Principal?.FindFirstValue(ClaimTypes.Role)) context.Fail("Account is unavailable.");
    } };
});
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx => RateLimitPartition.GetFixedWindowLimiter(
        ctx.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new() { PermitLimit = 120, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
    options.AddPolicy("auth", ctx => RateLimitPartition.GetFixedWindowLimiter(ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new() { PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});
builder.Services.AddControllers().ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = ctx =>
    new BadRequestObjectResult(new { success = false, error = new { code = "VALIDATION_ERROR", message = "Check your submitted fields.",
        fields = ctx.ModelState.Where(x => x.Value?.Errors.Count > 0).ToDictionary(x => x.Key, _ => "Invalid value.") }, requestId = ctx.HttpContext.TraceIdentifier }));
builder.Services.AddOpenApi();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseHsts(); app.UseHttpsRedirection(); }
app.Use(async (ctx, next) =>
{
    var started = System.Diagnostics.Stopwatch.StartNew();
    ctx.Response.Headers["X-Request-ID"] = ctx.TraceIdentifier;
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers.CacheControl = "no-store";
    try { await next(); }
    catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested) { }
    catch (Exception ex)
    {
        var business = ex as BusinessException;
        var postgres = ex as PostgresException ?? (ex as DbUpdateException)?.InnerException as PostgresException;
        var conflict = postgres?.SqlState is "23505" or "23P01" or "40001";
        var dependency = ex is NpgsqlException || ex.InnerException is NpgsqlException;
        ctx.Response.StatusCode = business?.Status ?? (conflict ? 409 : dependency ? 503 : 500);
        var code = business?.Code ?? (conflict ? "CONFLICT" : dependency ? "DATABASE_UNAVAILABLE" : "INTERNAL_ERROR");
        var message = business?.Message ?? (conflict ? "The record changed or the slot is no longer available. Refresh and retry." : "The service could not complete your request. Please retry.");
        app.Logger.LogWarning("Request failed with {Code}; request {RequestId}", code, ctx.TraceIdentifier);
        await ctx.Response.WriteAsJsonAsync(new { success = false, error = new { code, message }, requestId = ctx.TraceIdentifier });
    }
    finally { app.Logger.LogInformation("HTTP {Method} {Path} {Status} in {ElapsedMs}ms request {RequestId}", ctx.Request.Method, ctx.Request.Path.Value, ctx.Response.StatusCode, started.ElapsedMilliseconds, ctx.TraceIdentifier); }
});
app.UseStatusCodePages(async ctx =>
{
    var status = ctx.HttpContext.Response.StatusCode;
    await ctx.HttpContext.Response.WriteAsJsonAsync(new { success = false, error = new { code = $"HTTP_{status}", message = status switch { 401 => "Sign in to continue.", 403 => "You do not have permission.", 429 => "Too many requests. Try again in a minute.", _ => "Request could not be completed." } }, requestId = ctx.HttpContext.TraceIdentifier });
});
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" }));
app.MapGet("/health", () => Results.Ok(new { status = "alive" }));
app.MapGet("/health/ready", async (MarketplaceDb db, CancellationToken ct) =>
{
    try { return await db.Database.CanConnectAsync(ct) ? Results.Ok(new { status = "ready" }) : Results.StatusCode(503); }
    catch { return Results.StatusCode(503); }
});
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapControllers();
app.Run();
public partial class Program;
