using System.Net;
using System.Text;

namespace AgroTemp.API.Middleware
{
    /// <summary>
    /// Protects the /swagger route with HTTP Basic Authentication.
    /// Credentials are read from environment variables:
    ///   SWAGGER_USERNAME (default: "admin")
    ///   SWAGGER_PASSWORD (required — no default)
    /// </summary>
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _username;
        private readonly string _password;

        public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _username = configuration["Swagger:Username"] ?? "admin";
            _password = configuration["Swagger:Password"] ?? string.Empty;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only protect /swagger routes
            if (!context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // If no password is configured, block access entirely in production
            if (string.IsNullOrEmpty(_password))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            string? authHeader = context.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encodedCreds = authHeader["Basic ".Length..].Trim();
                var decodedCreds = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCreds));
                var parts = decodedCreds.Split(':', 2);

                if (parts.Length == 2 && parts[0] == _username && parts[1] == _password)
                {
                    await _next(context);
                    return;
                }
            }

            // Prompt browser for credentials
            context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"AgroTemp Swagger\", charset=\"UTF-8\"";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}
