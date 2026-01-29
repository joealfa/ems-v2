namespace EmployeeManagementSystem.Application.DTOs;

/// <summary>
/// Base record for all response DTOs with read-only display identifier and audit fields.
/// </summary>
public abstract record BaseResponseDto
{
    /// <summary>
    /// Gets the public-facing display identifier (12-digit number).
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedOn { get; init; }

    /// <summary>
    /// Gets the user who created the entity.
    /// </summary>
    public string CreatedBy { get; init; } = string.Empty;

    /// <summary>
    /// Gets the date and time when the entity was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; init; }

    /// <summary>
    /// Gets the user who last modified the entity.
    /// </summary>
    public string? ModifiedBy { get; init; }
}
