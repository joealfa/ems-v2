using EmployeeManagementSystem.Gateway.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add HTTP context accessor for token forwarding
builder.Services.AddHttpContextAccessor();

// Add gateway services (Redis, ApiClient, GraphQL)
builder.Services.AddGatewayServices(builder.Configuration);

// Add controllers for document proxy endpoints
builder.Services.AddControllers();

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

// Map controllers for document proxy endpoints
app.MapControllers();

// Map GraphQL endpoint
app.MapGraphQL();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
