using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Base class for all domain entities providing unique identification and creation tracking.
/// </summary>
public abstract class BaseEntity : AuditableEntity
{
    private static readonly Random _random = new();
    private const long MinDisplayId = 100000000000L; // 12-digit minimum
    private const long MaxDisplayId = 999999999999L; // 12-digit maximum

    /// <summary>
    /// Gets or sets the internal unique identifier for the entity.
    /// This is for internal use only and should not be exposed via API.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the public-facing display identifier (12-digit number).
    /// This is a randomly generated unique identifier.
    /// Uniqueness is enforced via database unique constraint.
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
        DisplayId = GenerateRandomDisplayId();
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Generates a random 12-digit display identifier.
    /// </summary>
    /// <returns>A random 12-digit number.</returns>
    private static long GenerateRandomDisplayId()
    {
        // Generate a random 12-digit number between 100000000000 and 999999999999
        byte[] buffer = new byte[8];
        _random.NextBytes(buffer);
        long randomValue = BitConverter.ToInt64(buffer, 0);
        
        // Ensure positive and within 12-digit range
        return MinDisplayId + (Math.Abs(randomValue) % (MaxDisplayId - MinDisplayId + 1));
    }
}
