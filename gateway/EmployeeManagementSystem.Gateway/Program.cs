using EmployeeManagementSystem.Gateway.Extensions;
using EmployeeManagementSystem.Gateway.Middleware;
using Serilog;
using Serilog.Events;

// Configure Serilog early to capture startup errors
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up Employee Management System Gateway");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add Serilog to the application
    _ = builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithEnvironmentName());

    // Add HTTP context accessor for token forwarding in GraphQL mutations
    _ = builder.Services.AddHttpContextAccessor();

    // Add HttpClient for REST proxy controllers
    _ = builder.Services.AddHttpClient();

    // Add controllers for REST endpoints (profile images, file downloads)
    _ = builder.Services.AddControllers();

    // Add gateway services (Redis, ApiClient, GraphQL)
    _ = builder.Services.AddGatewayServices(builder.Configuration);

    // Get CORS configuration
    string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? throw new InvalidOperationException("Cors:AllowedOrigins is not configured");

    // Configure CORS
    _ = builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowHost", policy =>
        {
            _ = policy
                .WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    WebApplication app = builder.Build();

    // Add security headers (CSP, HSTS, X-Frame-Options, etc.)
    _ = app.UseMiddleware<SecurityHeadersMiddleware>();

    _ = app.UseCors("AllowHost");

    // Inject access token from HttpOnly cookie into Authorization header
    _ = app.UseMiddleware<CookieAuthMiddleware>();

    // Add Serilog request logging - replaces default logging with structured logging
    _ = app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode > 499
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode > 399
                    ? LogEventLevel.Warning
                    : LogEventLevel.Information;

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());

            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
            }
        };
    });

    // Map REST controllers for file operations
    _ = app.MapControllers();

    // Enable WebSockets for GraphQL subscriptions
    _ = app.UseWebSockets();

    // Map GraphQL endpoint with subscriptions enabled
    _ = app.MapGraphQL();

    // Health check endpoint
    _ = app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

    Log.Information("Employee Management System Gateway started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Gateway application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
