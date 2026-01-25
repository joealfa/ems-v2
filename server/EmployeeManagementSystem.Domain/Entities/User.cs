namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents an authenticated user from Google OAuth2.
/// </summary>
public class User : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the Google OAuth2 subject identifier (unique per Google account).
    /// </summary>
    public string GoogleId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address from Google.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's first name from Google profile.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name from Google profile.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's profile picture URL from Google.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the user's role in the system.
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Gets or sets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the user's last login.
    /// </summary>
    public DateTime? LastLoginOn { get; set; }

    /// <summary>
    /// Gets or sets the collection of refresh tokens associated with this user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
