using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Infrastructure.Data;
using EmployeeManagementSystem.Infrastructure.Repositories;
using EmployeeManagementSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Get JWT configuration
IConfigurationSection jwtConfig = builder.Configuration.GetSection("Authentication:Jwt");
string jwtSecret = jwtConfig["Secret"] ?? throw new InvalidOperationException("Authentication:Jwt:Secret is not configured");
string jwtIssuer = jwtConfig["Issuer"] ?? throw new InvalidOperationException("Authentication:Jwt:Issuer is not configured");
string jwtAudience = jwtConfig["Audience"] ?? throw new InvalidOperationException("Authentication:Jwt:Audience is not configured");

// Get Google OAuth2 configuration
IConfigurationSection googleConfig = builder.Configuration.GetSection("Authentication:Google");
string googleClientId = googleConfig["ClientId"] ?? throw new InvalidOperationException("Authentication:Google:ClientId is not configured");
string googleClientSecret = googleConfig["ClientSecret"] ?? throw new InvalidOperationException("Authentication:Google:ClientSecret is not configured");

// Configure JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure OpenAPI with JWT Bearer and Google OAuth2 Authorization
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    // Use OpenAPI 3.1 specification
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;

    // Transform enum schemas to use string values (matching JsonStringEnumConverter runtime behavior)
    _ = options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        // Check if this is an enum type
        if (context.JsonTypeInfo.Type.IsEnum)
        {
            // Get enum values as strings
            string[] enumNames = Enum.GetNames(context.JsonTypeInfo.Type);

            // Clear integer-based enum definition and use string
            schema.Enum?.Clear();
            schema.Type = JsonSchemaType.String;

            // Add string enum values using JsonNode
            foreach (string name in enumNames)
            {
                schema.Enum ??= [];
                schema.Enum.Add(System.Text.Json.Nodes.JsonValue.Create(name));
            }
        }
        return Task.CompletedTask;
    });

    _ = options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // Set API metadata for better client generation
        document.Info.Title = "Employee Management System API";
        document.Info.Version = "v1";
        document.Info.Description = "REST API for the Employee Management System. Provides endpoints for managing persons, schools, employments, positions, salary grades, items, and documents.";
        document.Info.Contact = new OpenApiContact
        {
            Name = "EMS Development Team"
        };

        // Add server URLs for different environments
        document.Servers =
        [
            new OpenApiServer { Url = "https://localhost:5001", Description = "Development server" },
            new OpenApiServer { Url = "https://ems-api.example.com", Description = "Production server" }
        ];

        // Define JWT Bearer and Google OAuth2 security schemes
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. You can obtain a token by clicking 'Authorize' with Google OAuth2 below."
            },
            ["Google"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "Login with Google to get a JWT token",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                        TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID Connect" },
                            { "email", "Access to email" },
                            { "profile", "Access to profile" }
                        }
                    }
                }
            }
        };

        // Apply JWT Bearer security requirement to all operations
        if (document.Paths != null)
        {
            foreach (IOpenApiPathItem path in document.Paths.Values)
            {
                if (path.Operations == null)
                {
                    continue;
                }

                foreach (KeyValuePair<HttpMethod, OpenApiOperation> operation in path.Operations)
                {
                    operation.Value.Security ??= [];
                    operation.Value.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
                }
            }
        }

        return Task.CompletedTask;
    });
});

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("EmployeeManagementSystem.Infrastructure")));

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Services
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<ISalaryGradeService, SalaryGradeService>();
builder.Services.AddScoped<IEmploymentService, EmploymentService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Get CORS configuration
string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? throw new InvalidOperationException("Cors:AllowedOrigins is not configured");

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowHost", policy =>
    {
        _ = policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwaggerUI(options =>
    {
        // Point SwaggerUI to the OpenAPI endpoint
        options.SwaggerEndpoint("/openapi/v1.json", "Employee Management System API v1");
        options.RoutePrefix = "swagger";

        // Configure OAuth2 for Swagger UI - auto-populate credentials
        options.OAuthClientId(googleClientId);
        options.OAuthClientSecret(googleClientSecret);
        options.OAuthAppName("Employee Management System");
        options.OAuthUsePkce();
        options.OAuthScopes("openid", "email", "profile");
        options.OAuthScopeSeparator(" ");
    });

    // Apply migrations and seed data in development
    using IServiceScope scope = app.Services.CreateScope();
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    await DataSeeder.SeedAsync(dbContext);
}

// CORS must be before HttpsRedirection to handle preflight requests
app.UseCors("AllowHost");

// OpenAPI should be accessible via HTTP in development
if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
