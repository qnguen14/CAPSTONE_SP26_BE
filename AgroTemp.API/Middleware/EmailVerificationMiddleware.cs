using AgroTemp.Domain.Context;
using AgroTemp.Domain.Entities;
using AgroTemp.Repository.Interfaces;
using System.Security.Claims;

namespace AgroTemp.API.Middleware;

public class EmailVerificationMiddleware
{
    private readonly RequestDelegate _next;

    // Paths that should bypass the verification check
    private static readonly HashSet<string> _bypassPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/v1/login",
        "/api/v1/register",
        "/api/v1/google-login",
        "/api/v1/verify-email",
        "/api/v1/resend-verification",
        "/api/v1/forget",
        "/api/v1/reset",
        "/api/v1/payment/verify",   // PayOS webhook
        "/api/v1/payment/callback", // PayOS callback
    };

    public EmailVerificationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork<AgroTempDbContext> unitOfWork)
    {
        // Only check authenticated requests
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Skip bypass paths and swagger paths
            var shouldBypass = _bypassPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                || path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("/hubs", StringComparison.OrdinalIgnoreCase);

            if (!shouldBypass)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var user = await unitOfWork.GetRepository<User>()
                        .FirstOrDefaultAsync(predicate: u => u.Id == userId);

                    if (user != null && !user.IsVerified)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            StatusCode = StatusCodes.Status403Forbidden,
                            Message = "Email not verified. Please verify your email to continue.",
                            Data = (object?)null
                        });
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
