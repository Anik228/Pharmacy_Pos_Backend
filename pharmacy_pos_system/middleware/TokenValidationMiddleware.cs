using pharmacy_pos_system.module.user.service;

namespace pharmacy_pos_system.middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var token = await userService.GetCurrentToken("Authorization");

            if (!string.IsNullOrEmpty(token))
            {
                var isBlacklisted = await userService.IsTokenBlacklisted(token);
                if (isBlacklisted)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { message = "Token has been invalidated. Please login again." });
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class TokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }
} 