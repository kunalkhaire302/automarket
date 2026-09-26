using AutoMarket.Domain;
using AutoMarket.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SupabasePostgres")
    ?? throw new InvalidOperationException("Database configuration missing.");
try
{
    if (args.FirstOrDefault() == "bootstrap-admin")
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            throw new InvalidOperationException("Admin bootstrap is restricted to Development.");
        var email = Environment.GetEnvironmentVariable("BootstrapAdmin__Email")?.Trim().ToLowerInvariant();
        var password = Environment.GetEnvironmentVariable("BootstrapAdmin__Password");
        if (string.IsNullOrWhiteSpace(email) || password is null || password.Length < 16)
            throw new InvalidOperationException("Configure a bootstrap email and password of at least 16 characters.");
        var options = new DbContextOptionsBuilder<MarketplaceDb>().UseNpgsql(connectionString).Options;
        await using var db = new MarketplaceDb(options);
        var user = await db.Set<User>().FirstOrDefaultAsync(x => x.Email == email);
        if (user is null)
        {
            user = new User { Name = "AutoMarket Test Administrator", Email = email, Role = "ADMIN", IsActive = true, EmailVerified = true };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, password);
            db.Add(user);
        }
        else
        {
            user.Role = "ADMIN";
            user.IsActive = true;
        }
        await db.SaveChangesAsync();
        Console.WriteLine($"Development administrator ready: {user.Id}");
        return;
    }
    if (args.FirstOrDefault() == "seed-demo")
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Demo seeding is restricted to Development.");
        var options = new DbContextOptionsBuilder<MarketplaceDb>().UseNpgsql(connectionString).Options;
        await using var db = new MarketplaceDb(options);
        var report = await DemoInventorySeeder.SeedAsync(db);
        Console.WriteLine($"Demo inventory ready: {report.Cars} cars, {report.Cities} cities, {report.Centers} centers, {report.Images} image records, {report.Featured} featured.");
        return;
    }
    if (args.FirstOrDefault() == "inspect-demo-photos")
    {
        var options = new DbContextOptionsBuilder<MarketplaceDb>().UseNpgsql(connectionString).Options;
        await using var db = new MarketplaceDb(options);
        var licensed = await db.Set<CarImage>().CountAsync(x => x.PublicUrl != null && x.SourceUrl != null && x.Attribution != null);
        var total = await db.Set<CarImage>().CountAsync(x => x.StorageKey.StartsWith("demo/"));
        Console.WriteLine($"Demo image metadata: {licensed} licensed primary photos, {total} demo image records.");
        return;
    }
    if (args.FirstOrDefault() == "reconcile-demo-migration")
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Migration reconciliation is restricted to Development.");
        var options = new DbContextOptionsBuilder<MarketplaceDb>().UseNpgsql(connectionString).Options;
        await using var db = new MarketplaceDb(options);
        const string id = "20260926120542_DemoInventory";
        await db.Database.ExecuteSqlRawAsync("insert into automarket.\"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") values ({0}, {1}) on conflict (\"MigrationId\") do nothing", id, "10.0.4");
        Console.WriteLine("Demo inventory migration history reconciled.");
        return;
    }
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    await using var command = new NpgsqlCommand("select table_schema, table_name from information_schema.tables where table_schema in ('public','automarket') order by table_schema, table_name", connection);
    await using var reader = await command.ExecuteReaderAsync();
    Console.WriteLine("Database connected over encrypted transport. Existing application/public tables:");
    while (await reader.ReadAsync()) Console.WriteLine($"{reader.GetString(0)}.{reader.GetString(1)}");
}
catch (Exception e)
{
    Console.Error.WriteLine($"Database inspection failed: {e.GetType().Name}. No credentials logged.");
    if (e is PostgresException p) Console.Error.WriteLine($"PostgreSQL code: {p.SqlState}");
    else if (e.InnerException is not null) Console.Error.WriteLine($"Cause: {e.InnerException.GetType().Name}: {e.InnerException.Message}");
    Environment.ExitCode = 1;
}
