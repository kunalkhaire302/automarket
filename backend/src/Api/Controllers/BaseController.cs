using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AutoMarket.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected Guid Actor => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    protected bool IsAdmin => User.IsInRole("ADMIN");
    protected object Envelope(object? data, object? meta = null) => new { success = true, data, message = "Operation completed successfully", meta, requestId = HttpContext.TraceIdentifier };
}
