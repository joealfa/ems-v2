using EmployeeManagementSystem.Application.DTOs.Auth;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for authentication operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user using a Google OAuth2 ID token.
    /// Creates a new user if one doesn't exist.
    /// </summary>
    /// <param name="idToken">The Google ID token to validate.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <returns>Authentication response with tokens and user info.</returns>
    Task<AuthResponseDto> AuthenticateWithGoogleAsync(string idToken, string ipAddress);

    /// <summary>
    /// Authenticates a user using a Google OAuth2 access token.
    /// Fetches user info from Google and creates/updates user.
    /// </summary>
    /// <param name="accessToken">The Google access token.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <returns>Authentication response with tokens and user info.</returns>
    Task<AuthResponseDto> AuthenticateWithGoogleAccessTokenAsync(string accessToken, string ipAddress);

    /// <summary>
    /// Refreshes an access token using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to use.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <returns>Authentication response with new tokens.</returns>
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, string ipAddress);

    /// <summary>
    /// Revokes a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to revoke.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <returns>True if the token was revoked, false otherwise.</returns>
    Task<bool> RevokeTokenAsync(string refreshToken, string ipAddress);

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>The user DTO if found, null otherwise.</returns>
    Task<UserDto?> GetUserByIdAsync(Guid userId);
}
