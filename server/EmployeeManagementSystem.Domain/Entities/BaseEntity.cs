using EmployeeManagementSystem.Domain.Common;
using EmployeeManagementSystem.Domain.Events;
using System.ComponentModel.DataAnnotations;

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

    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the domain events for this entity.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

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

    /// <summary>
    /// Adds a domain event to this entity.
    /// </summary>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from this entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
