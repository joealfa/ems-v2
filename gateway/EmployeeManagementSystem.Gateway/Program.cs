using EmployeeManagementSystem.Gateway.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add HTTP context accessor for token forwarding in GraphQL mutations
builder.Services.AddHttpContextAccessor();

// Add HttpClient for REST proxy controllers
builder.Services.AddHttpClient();

// Add controllers for REST endpoints (profile images, file downloads)
builder.Services.AddControllers();

// Add gateway services (Redis, ApiClient, GraphQL)
builder.Services.AddGatewayServices(builder.Configuration);

// Get CORS configuration
string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? throw new InvalidOperationException("Cors:AllowedOrigins is not configured");

// Configure CORS
builder.Services.AddCors(options =>
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

app.UseCors("AllowHost");

// Map REST controllers for file operations
app.MapControllers();

// Map GraphQL endpoint
app.MapGraphQL();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
