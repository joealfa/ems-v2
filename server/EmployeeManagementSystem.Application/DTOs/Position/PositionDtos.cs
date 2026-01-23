using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Application.DTOs.Position;

/// <summary>
/// DTO for creating a new position.
/// </summary>
public class CreatePositionDto
{
    /// <summary>
    /// Gets or sets the title name of the position.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TitleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the position.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating an existing position.
/// </summary>
public class UpdatePositionDto
{
    /// <summary>
    /// Gets or sets the title name of the position.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TitleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the position.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this position is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for position response.
/// </summary>
public class PositionResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets or sets the title name of the position.
    /// </summary>
    public string TitleName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the position.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this position is active.
    /// </summary>
    public bool IsActive { get; init; }
}
