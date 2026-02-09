namespace EmployeeManagementSystem.Gateway.Middleware;

/// <summary>
/// Middleware that reads the access token from an HttpOnly cookie
/// and injects it as a Bearer token in the Authorization header.
/// This enables cookie-based authentication while keeping the downstream
/// token-forwarding logic (to the backend API) unchanged.
/// </summary>
public class CookieAuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization")
            && context.Request.Cookies.TryGetValue("accessToken", out string? accessToken)
            && !string.IsNullOrWhiteSpace(accessToken))
        {
            context.Request.Headers.Authorization = $"Bearer {accessToken}";
        }

        await next(context);
    }
}
