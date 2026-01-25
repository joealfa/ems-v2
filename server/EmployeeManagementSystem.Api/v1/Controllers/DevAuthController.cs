#if DEBUG
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// Development-only controller for generating JWT tokens without OAuth authentication.
/// This controller is only available in DEBUG builds and should NOT be used in production.
/// </summary>
[ApiController]
[Route("api/v1/dev/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "development")]
public class DevAuthController(IConfiguration configuration, ILogger<DevAuthController> logger) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<DevAuthController> _logger = logger;

    /// <summary>
    /// Generates a development JWT token for testing purposes.
    /// </summary>
    /// <param name="request">Optional user information to include in the token.</param>
    /// <returns>A JWT token for development use.</returns>
    [HttpPost("token")]
    [ProducesResponseType(typeof(DevTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GenerateToken([FromBody] DevTokenRequest? request)
    {
        try
        {
            request ??= new DevTokenRequest(null, null, null);

            string userId = request.UserId ?? "dev-user-123";
            string email = request.Email ?? "dev@example.com";
            string name = request.Name ?? "Dev User";

            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Name, name),
                new("sub", userId),
                new("email", email),
                new("name", name),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            IConfigurationSection jwtConfig = _configuration.GetSection("Authentication:Jwt");
            string secret = jwtConfig["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
            string issuer = jwtConfig["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            string audience = jwtConfig["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secret));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);
            DateTime expiry = DateTime.UtcNow.AddHours(8);

            JwtSecurityToken token = new(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiry,
                signingCredentials: creds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogWarning("Development token generated for user: {Email}", email);

            return Ok(new DevTokenResponse
            {
                Token = tokenString,
                ExpiresAt = expiry,
                UserId = userId,
                Email = email,
                Name = name
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating development token");
            return BadRequest(new { message = "Failed to generate token", error = ex.Message });
        }
    }

    /// <summary>
    /// Gets information about the current token (if authenticated).
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized(new { message = "Not authenticated" });
        }

        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        string? name = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new
        {
            userId,
            email,
            name,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}

/// <summary>
/// Request model for generating a development token.
/// </summary>
public record DevTokenRequest(string? UserId, string? Email, string? Name);

/// <summary>
/// Response model for a generated development token.
/// </summary>
public class DevTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
#endif
