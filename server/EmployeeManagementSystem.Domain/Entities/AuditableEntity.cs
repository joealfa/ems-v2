namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Base class for auditable entities providing modification tracking and soft delete functionality.
/// </summary>
public abstract class AuditableEntity
{
    /// <summary>
    /// Gets or sets the user who last modified the entity.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
