namespace EmployeeManagementSystem.Application.DTOs.Auth;

/// <summary>
/// Request DTO for Google OAuth2 authentication using ID token.
/// </summary>
public class GoogleAuthRequestDto
{
    /// <summary>
    /// The ID token received from Google OAuth2 sign-in.
    /// </summary>
    public string IdToken { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for Google OAuth2 authentication using access token (from Swagger OAuth2).
/// </summary>
public class GoogleAccessTokenRequestDto
{
    /// <summary>
    /// The access token received from Google OAuth2 authorization code flow.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for refreshing an access token.
/// </summary>
public class RefreshTokenRequestDto
{
    /// <summary>
    /// The refresh token to use for obtaining a new access token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO containing authentication tokens and user information.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// The JWT access token for API authentication.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// The refresh token for obtaining new access tokens.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the access token expires.
    /// </summary>
    public DateTime ExpiresOn { get; set; }

    /// <summary>
    /// The authenticated user's information.
    /// </summary>
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// DTO representing user information.
/// </summary>
public class UserDto
{
    /// <summary>
    /// The user's unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The user's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The user's profile picture URL.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// The user's role in the system.
    /// </summary>
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO for revoking a refresh token.
/// </summary>
public class RevokeTokenRequestDto
{
    /// <summary>
    /// The refresh token to revoke.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
