using AgroTemp.Domain.Context;
using AgroTemp.Domain.Entities;
using AgroTemp.Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace AgroTemp.API.Middleware;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public TokenBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork<AgroTempDbContext> unitOfWork)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if(!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            
            var handler = new JwtSecurityTokenHandler();
            if(handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                var jti = jwtToken.Id;

                if(string.IsNullOrEmpty(jti))
                {
                    jti = Convert.ToBase64String(
                        System.Security.Cryptography.SHA256.HashData(
                            System.Text.Encoding.UTF8.GetBytes(token)));
                }

                var blacklistedTokens = await unitOfWork.GetRepository<BlacklistedToken>()
                .GetListAsync(predicate: x => x.TokenId == jti);

                if(blacklistedTokens.Any())
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Token has been revoked. Please login again.",
                        Data = (object?)null
                    });
                    return; 
                }
            }
        }

        await _next(context);   
    }
}
