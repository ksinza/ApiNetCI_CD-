using System;
using System.Text;
using Microsoft.Extensions.Options;
using WebApplication1.Helper;

namespace WebApplication1.Middlewares;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    private readonly UserAuthSettings _userAuthSettings;
    private  string _user = string.Empty;
    private  string _password = string.Empty;

    public BasicAuthMiddleware(RequestDelegate next, IOptions<UserAuthSettings> userAuthSettings)
    {
        _next = next;
        _userAuthSettings = userAuthSettings.Value;

    }
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        // Omitir Swagger/Scalar/OpenAPI para poder usar la documentación
        if (path.Contains("swagger", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("scalar", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("openapi", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Authorization", out var header) ||
            !header.ToString().StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var token = header.ToString().Substring("Basic ".Length).Trim();
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var parts = decoded.Split(':', 2);
        var user = parts.ElementAtOrDefault(0);
        var pass = parts.ElementAtOrDefault(1);

        if (user == _userAuthSettings.User && pass == _userAuthSettings.Password)
            await _next(context);
        else
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }
}

public static class BasicAuthExtensions
{
    public static IApplicationBuilder UseBasicAuth(this IApplicationBuilder app)
        => app.UseMiddleware<BasicAuthMiddleware>();
}