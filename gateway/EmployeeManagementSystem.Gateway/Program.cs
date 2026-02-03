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

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        _ = policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

WebApplication app = builder.Build();

app.UseCors();

// Map REST controllers for file operations
app.MapControllers();

// Map GraphQL endpoint
app.MapGraphQL();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
