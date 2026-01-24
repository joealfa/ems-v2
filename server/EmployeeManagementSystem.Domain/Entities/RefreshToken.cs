namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents a refresh token for JWT authentication.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Gets or sets the unique identifier for the refresh token.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the token value.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the token expires.
    /// </summary>
    public DateTime ExpiresOn { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the token was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the token was created.
    /// </summary>
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the token was revoked.
    /// </summary>
    public DateTime? RevokedOn { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the token was revoked.
    /// </summary>
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Gets or sets the token that replaced this one.
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Gets or sets the reason for revocation.
    /// </summary>
    public string? ReasonRevoked { get; set; }

    /// <summary>
    /// Gets a value indicating whether the token is expired.
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;

    /// <summary>
    /// Gets a value indicating whether the token has been revoked.
    /// </summary>
    public bool IsRevoked => RevokedOn != null;

    /// <summary>
    /// Gets a value indicating whether the token is active (not expired and not revoked).
    /// </summary>
    public bool IsActive => !IsRevoked && !IsExpired;

    /// <summary>
    /// Gets or sets the user identifier this token belongs to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user this token belongs to.
    /// </summary>
    public User User { get; set; } = null!;
}
