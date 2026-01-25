using EmployeeManagementSystem.Application.DTOs.Auth;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

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
            return BadRequest(new { message = "ID token is required" });
        }

        try
        {
            var ipAddress = GetIpAddress();
            var result = await _authService.AuthenticateWithGoogleAsync(request.IdToken, ipAddress);
            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(result);
        }
        catch (Google.Apis.Auth.InvalidJwtException)
        {
            return Unauthorized(new { message = "Invalid Google ID token" });
        }
        catch (Exception ex)
        {
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
            return BadRequest(new { message = "Access token is required" });
        }

        try
        {
            var ipAddress = GetIpAddress();
            var result = await _authService.AuthenticateWithGoogleAccessTokenAsync(request.AccessToken, ipAddress);
            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
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
        var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token is required" });
        }

        var ipAddress = GetIpAddress();
        var result = await _authService.RefreshTokenAsync(refreshToken, ipAddress);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        SetRefreshTokenCookie(result.RefreshToken);
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
        var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest(new { message = "Refresh token is required" });
        }

        var ipAddress = GetIpAddress();
        var result = await _authService.RevokeTokenAsync(refreshToken, ipAddress);

        if (!result)
        {
            return BadRequest(new { message = "Token not found or already revoked" });
        }

        // Clear the refresh token cookie
        Response.Cookies.Delete("refreshToken");

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
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var user = await _authService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Sets the refresh token in an HTTP-only cookie.
    /// </summary>
    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
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
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim() ?? "Unknown";
        }

        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
    }
}
