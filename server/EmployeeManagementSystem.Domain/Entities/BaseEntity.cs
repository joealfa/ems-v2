using System.ComponentModel.DataAnnotations;
using EmployeeManagementSystem.Domain.Common;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Base class for all domain entities providing unique identification and creation tracking.
/// </summary>
public abstract class BaseEntity : AuditableEntity
{
    /// <summary>
    /// Gets or sets the internal unique identifier for the entity.
    /// This is for internal use only and should not be exposed via API.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the public-facing display identifier (12-digit number).
    /// Generated using Snowflake-style algorithm for guaranteed uniqueness.
    /// Uniqueness is also enforced via database unique constraint.
    /// </summary>
    public long DisplayId { get; private set; }

    /// <summary>
    /// Gets or sets the user who created the entity.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    [Required]
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        DisplayId = SnowflakeIdGenerator.GenerateId();
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Regenerates the DisplayId with a new unique value.
    /// Used for retry logic when a collision occurs.
    /// </summary>
    public void RegenerateDisplayId()
    {
        DisplayId = SnowflakeIdGenerator.GenerateId();
    }
}
