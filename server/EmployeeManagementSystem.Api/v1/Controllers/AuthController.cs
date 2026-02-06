using EmployeeManagementSystem.Api.Controllers;
using EmployeeManagementSystem.Application.DTOs.Auth;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for authentication operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthController"/> class.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ApiControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// Authenticates a user using Google OAuth2 ID token.
    /// </summary>
    /// <param name="request">The Google authentication request containing the ID token.</param>
    /// <returns>Authentication response with access and refresh tokens.</returns>
    [HttpPost("google")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleAuthRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
        {
            _logger.LogWarning("Google login attempt with empty ID token");
            return BadRequest(new { message = "ID token is required" });
        }

        try
        {
            string ipAddress = GetIpAddress();
            _logger.LogInformation("Google login attempt from IP: {IpAddress}", ipAddress);

            AuthResponseDto result = await _authService.AuthenticateWithGoogleAsync(request.IdToken, ipAddress);
            SetRefreshTokenCookie(result.RefreshToken);

            _logger.LogInformation("User {UserId} ({Email}) successfully authenticated via Google from IP {IpAddress}",
                result.User.Id, result.User.Email, ipAddress);

            return Ok(result);
        }
        catch (Google.Apis.Auth.InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Invalid Google ID token received from IP {IpAddress}", GetIpAddress());
            return Unauthorized(new { message = "Invalid Google ID token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google authentication from IP {IpAddress}", GetIpAddress());
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates a user using Google OAuth2 access token (for Swagger UI).
    /// Use this endpoint after authenticating with Google via Swagger's OAuth2 flow.
    /// </summary>
    /// <param name="request">The Google access token request.</param>
    /// <returns>Authentication response with JWT access and refresh tokens.</returns>
    [HttpPost("google/token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> GoogleAccessTokenLogin([FromBody] GoogleAccessTokenRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.AccessToken))
        {
            _logger.LogWarning("Google access token login attempt with empty access token");
            return BadRequest(new { message = "Access token is required" });
        }

        try
        {
            string ipAddress = GetIpAddress();
            _logger.LogInformation("Google access token login attempt from IP: {IpAddress}", ipAddress);

            AuthResponseDto result = await _authService.AuthenticateWithGoogleAccessTokenAsync(request.AccessToken, ipAddress);
            SetRefreshTokenCookie(result.RefreshToken);

            _logger.LogInformation("User {UserId} ({Email}) successfully authenticated via Google access token from IP {IpAddress}",
                result.User.Id, result.User.Email, ipAddress);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google access token authentication from IP {IpAddress}", GetIpAddress());
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    /// <param name="request">The refresh token request. If not provided, uses the cookie.</param>
    /// <returns>New authentication tokens.</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto? request)
    {
        string? refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("Token refresh attempt without refresh token from IP {IpAddress}", GetIpAddress());
            return Unauthorized(new { message = "Refresh token is required" });
        }

        string ipAddress = GetIpAddress();
        _logger.LogDebug("Token refresh attempt from IP {IpAddress}", ipAddress);

        AuthResponseDto? result = await _authService.RefreshTokenAsync(refreshToken, ipAddress);

        if (result == null)
        {
            _logger.LogWarning("Token refresh failed - invalid or expired token from IP {IpAddress}", ipAddress);
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        SetRefreshTokenCookie(result.RefreshToken);
        _logger.LogInformation("Token successfully refreshed for user {UserId} from IP {IpAddress}", result.User.Id, ipAddress);
        return Ok(result);
    }

    /// <summary>
    /// Revokes a refresh token (logout).
    /// </summary>
    /// <param name="request">The token to revoke. If not provided, uses the cookie.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequestDto? request)
    {
        string? refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("Token revoke attempt without refresh token from IP {IpAddress}", GetIpAddress());
            return BadRequest(new { message = "Refresh token is required" });
        }

        string ipAddress = GetIpAddress();
        string? userId = CurrentUser;

        bool result = await _authService.RevokeTokenAsync(refreshToken, ipAddress);

        if (!result)
        {
            _logger.LogWarning("Token revoke failed - token not found or already revoked for user {UserId} from IP {IpAddress}",
                userId, ipAddress);
            return BadRequest(new { message = "Token not found or already revoked" });
        }

        // Clear the refresh token cookie
        Response.Cookies.Delete("refreshToken");

        _logger.LogInformation("User {UserId} successfully revoked token (logout) from IP {IpAddress}", userId, ipAddress);

        return NoContent();
    }

    /// <summary>
    /// Gets the current authenticated user's information.
    /// </summary>
    /// <returns>The current user's information.</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        string? userIdClaim = CurrentUser;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        UserDto? user = await _authService.GetUserByIdAsync(userId);

        return user == null ? (ActionResult<UserDto>)Unauthorized(new { message = "User not found" }) : (ActionResult<UserDto>)Ok(user);
    }

    /// <summary>
    /// Sets the refresh token in an HTTP-only cookie.
    /// </summary>
    private void SetRefreshTokenCookie(string refreshToken)
    {
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    /// <summary>
    /// Gets the client's IP address.
    /// </summary>
    private string GetIpAddress()
    {
        return Request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwardedFor)
            ? forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ?? "Unknown"
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
    }
}
