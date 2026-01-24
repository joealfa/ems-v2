using System.Text;
using System.Text.Json.Serialization;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Infrastructure.Data;
using EmployeeManagementSystem.Infrastructure.Repositories;
using EmployeeManagementSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Get JWT configuration
var jwtConfig = builder.Configuration.GetSection("Authentication:Jwt");
var jwtSecret = jwtConfig["Secret"] ?? throw new InvalidOperationException("Authentication:Jwt:Secret is not configured");
var jwtIssuer = jwtConfig["Issuer"] ?? throw new InvalidOperationException("Authentication:Jwt:Issuer is not configured");
var jwtAudience = jwtConfig["Audience"] ?? throw new InvalidOperationException("Authentication:Jwt:Audience is not configured");

// Get Google OAuth2 configuration
var googleConfig = builder.Configuration.GetSection("Authentication:Google");
var googleClientId = googleConfig["ClientId"] ?? throw new InvalidOperationException("Authentication:Google:ClientId is not configured");
var googleClientSecret = googleConfig["ClientSecret"] ?? throw new InvalidOperationException("Authentication:Google:ClientSecret is not configured");

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

// Configure OpenAPI/Swagger with JWT Bearer and Google OAuth2 Authorization
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums();

    // Define JWT Bearer security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token. You can obtain a token by clicking 'Authorize' with Google OAuth2 below."
    });

    // Define Google OAuth2 security scheme for Swagger UI login
    options.AddSecurityDefinition("Google", new OpenApiSecurityScheme
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
    });

    // Apply JWT Bearer security requirement globally
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
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

// Configure CORS
// WARNING: This policy is for development only. 
// TODO: Restrict to specific origins before production deployment.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedHosts", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:7009")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Configure OAuth2 for Swagger UI - auto-populate credentials
        options.OAuthClientId(googleClientId);
        options.OAuthClientSecret(googleClientSecret);
        options.OAuthAppName("Employee Management System");
        options.OAuthUsePkce();
        options.OAuthScopes("openid", "email", "profile");
        options.OAuthScopeSeparator(" ");
    });
    
    // Apply migrations and seed data in development
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    await DataSeeder.SeedAsync(dbContext);
}

app.UseHttpsRedirection();
app.UseCors("AllowedHosts");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

