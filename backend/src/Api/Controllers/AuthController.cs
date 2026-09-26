using AutoMarket.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AutoMarket.Api.Controllers;

[Route("api/v1/auth")]
public sealed class AuthController(AuthService auth) : BaseController
{
    [HttpPost("register"), EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(RegisterRequest r, CancellationToken ct) => StatusCode(201, Envelope(await auth.Register(r, ct)));
    [HttpPost("login"), EnableRateLimiting("auth")]
    public async Task<object> Login(LoginRequest r, CancellationToken ct) => Envelope(await auth.Login(r, ct));
    [HttpPost("refresh"), EnableRateLimiting("auth")]
    public async Task<object> Refresh(TokenRequest r, CancellationToken ct) => Envelope(await auth.Refresh(r.RefreshToken, ct));
    [HttpPost("logout")]
    public async Task<object> Logout(TokenRequest r, CancellationToken ct) { await auth.Logout(r.RefreshToken, ct); return Envelope(new { signedOut = true }); }
    [HttpGet("me"), Authorize]
    public async Task<object> Me(CancellationToken ct) => Envelope(await auth.Me(Actor, ct));
}
