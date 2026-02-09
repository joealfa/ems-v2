namespace EmployeeManagementSystem.Api.Middleware;

/// <summary>
/// Middleware that adds security headers to all responses.
/// Includes Content Security Policy (CSP), HSTS, and other security headers.
/// </summary>
public class SecurityHeadersMiddleware(RequestDelegate next, IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Content Security Policy (CSP)
        // Restricts sources from which content can be loaded
        if (env.IsProduction())
        {
            // Production: Strict CSP
            context.Response.Headers.ContentSecurityPolicy =
                "default-src 'none'; " +
                "script-src 'self'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data:; " +
                "font-src 'self'; " +
                "connect-src 'self'; " +
                "base-uri 'self'; " +
                "form-action 'self'; " +
                "frame-ancestors 'none';";
        }
        else
        {
            // Development: Relaxed CSP for Swagger UI
            context.Response.Headers.ContentSecurityPolicy =
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data: https:; " +
                "font-src 'self' data:; " +
                "connect-src 'self'; " +
                "frame-ancestors 'none';";
        }

        // HTTP Strict Transport Security (HSTS)
        // Forces clients to use HTTPS for future requests
        if (env.IsProduction())
        {
            context.Response.Headers.StrictTransportSecurity = "max-age=31536000; includeSubDomains; preload";
        }

        // X-Content-Type-Options
        // Prevents MIME type sniffing
        context.Response.Headers.XContentTypeOptions = "nosniff";

        // X-Frame-Options
        // Prevents clickjacking attacks
        context.Response.Headers.XFrameOptions = "DENY";

        // X-XSS-Protection
        // Legacy header, but still good for older browsers
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

        // Referrer-Policy
        // Controls how much referrer information is included with requests
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Permissions-Policy (formerly Feature-Policy)
        // Controls which browser features and APIs can be used
        context.Response.Headers["Permissions-Policy"] =
            "geolocation=(), " +
            "microphone=(), " +
            "camera=(), " +
            "payment=(), " +
            "usb=(), " +
            "magnetometer=(), " +
            "gyroscope=(), " +
            "accelerometer=()";

        // Remove X-Powered-By header (security through obscurity)
        context.Response.Headers.Remove("X-Powered-By");

        await next(context);
    }
}
