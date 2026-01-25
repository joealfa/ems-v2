using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace EmployeeManagementSystem.Infrastructure.Data;

/// <summary>
/// The main database context for the Employee Management System.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
/// </remarks>
/// <param name="options">The database context options.</param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{

    /// <summary>
    /// Gets or sets the Persons DbSet.
    /// </summary>
    public DbSet<Person> Persons => Set<Person>();

    /// <summary>
    /// Gets or sets the Addresses DbSet.
    /// </summary>
    public DbSet<Address> Addresses => Set<Address>();

    /// <summary>
    /// Gets or sets the Contacts DbSet.
    /// </summary>
    public DbSet<Contact> Contacts => Set<Contact>();

    /// <summary>
    /// Gets or sets the Schools DbSet.
    /// </summary>
    public DbSet<School> Schools => Set<School>();

    /// <summary>
    /// Gets or sets the Items DbSet.
    /// </summary>
    public DbSet<Item> Items => Set<Item>();

    /// <summary>
    /// Gets or sets the Positions DbSet.
    /// </summary>
    public DbSet<Position> Positions => Set<Position>();

    /// <summary>
    /// Gets or sets the SalaryGrades DbSet.
    /// </summary>
    public DbSet<SalaryGrade> SalaryGrades => Set<SalaryGrade>();

    /// <summary>
    /// Gets or sets the Employments DbSet.
    /// </summary>
    public DbSet<Employment> Employments => Set<Employment>();

    /// <summary>
    /// Gets or sets the EmploymentSchools DbSet.
    /// </summary>
    public DbSet<EmploymentSchool> EmploymentSchools => Set<EmploymentSchool>();

    /// <summary>
    /// Gets or sets the Documents DbSet.
    /// </summary>
    public DbSet<Document> Documents => Set<Document>();

    /// <summary>
    /// Gets or sets the Users DbSet.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets or sets the RefreshTokens DbSet.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// Configures the model using Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filter for soft delete (RefreshToken has its own filter configured separately)
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Skip RefreshToken as it has a custom query filter based on User.IsDeleted
            if (entityType.ClrType == typeof(RefreshToken))
            {
                continue;
            }

            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                _ = modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
            }
        }

        // Configure Person
        _ = modelBuilder.Entity<Person>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.MiddleName).HasMaxLength(100);
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            _ = entity.HasMany(e => e.Addresses)
                .WithOne(a => a.Person)
                .HasForeignKey(a => a.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasMany(e => e.Contacts)
                .WithOne(c => c.Person)
                .HasForeignKey(c => c.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasMany(e => e.Employments)
                .WithOne(emp => emp.Person)
                .HasForeignKey(emp => emp.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasMany(e => e.Documents)
                .WithOne(d => d.Person)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = entity.Property(e => e.ProfileImageUrl).HasMaxLength(2048);
        });

        // Configure Address
        _ = modelBuilder.Entity<Address>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.Address1).IsRequired().HasMaxLength(200);
            _ = entity.Property(e => e.Address2).HasMaxLength(200);
            _ = entity.Property(e => e.Barangay).HasMaxLength(100);
            _ = entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Province).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.ZipCode).HasMaxLength(20);
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure Contact
        _ = modelBuilder.Entity<Contact>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.Mobile).HasMaxLength(20);
            _ = entity.Property(e => e.LandLine).HasMaxLength(20);
            _ = entity.Property(e => e.Fax).HasMaxLength(20);
            _ = entity.Property(e => e.Email).HasMaxLength(256);
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure School
        _ = modelBuilder.Entity<School>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.SchoolName).IsRequired().HasMaxLength(200);
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            _ = entity.HasMany(e => e.Addresses)
                .WithOne(a => a.School)
                .HasForeignKey(a => a.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasMany(e => e.Contacts)
                .WithOne(c => c.School)
                .HasForeignKey(c => c.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Item
        _ = modelBuilder.Entity<Item>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.ItemName).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Description).HasMaxLength(500);
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure Position
        _ = modelBuilder.Entity<Position>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.TitleName).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Description).HasMaxLength(500);
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure SalaryGrade
        _ = modelBuilder.Entity<SalaryGrade>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.SalaryGradeName).IsRequired().HasMaxLength(50);
            _ = entity.Property(e => e.Description).HasMaxLength(500);
            _ = entity.Property(e => e.MonthlySalary).HasPrecision(18, 2);
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure Employment
        _ = modelBuilder.Entity<Employment>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.DepEdId).HasMaxLength(50);
            _ = entity.Property(e => e.PSIPOPItemNumber).HasMaxLength(50);
            _ = entity.Property(e => e.TINId).HasMaxLength(20);
            _ = entity.Property(e => e.GSISId).HasMaxLength(20);
            _ = entity.Property(e => e.PhilHealthId).HasMaxLength(20);
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            _ = entity.HasOne(e => e.Position)
                .WithMany(p => p.Employments)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasOne(e => e.SalaryGrade)
                .WithMany(sg => sg.Employments)
                .HasForeignKey(e => e.SalaryGradeId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasOne(e => e.Item)
                .WithMany(i => i.Employments)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure EmploymentSchool (Many-to-Many)
        _ = modelBuilder.Entity<EmploymentSchool>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            _ = entity.HasIndex(e => new { e.EmploymentId, e.SchoolId, e.StartDate }).IsUnique();

            _ = entity.HasOne(es => es.Employment)
                .WithMany(e => e.EmploymentSchools)
                .HasForeignKey(es => es.EmploymentId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = entity.HasOne(es => es.School)
                .WithMany(s => s.EmploymentSchools)
                .HasForeignKey(es => es.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Document
        _ = modelBuilder.Entity<Document>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.DisplayId).IsUnique();
            _ = entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            _ = entity.Property(e => e.FileExtension).IsRequired().HasMaxLength(10);
            _ = entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.BlobUrl).IsRequired().HasMaxLength(2048);
            _ = entity.Property(e => e.BlobName).IsRequired().HasMaxLength(500);
            _ = entity.Property(e => e.ContainerName).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Description).HasMaxLength(500);
            _ = entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure User
        _ = modelBuilder.Entity<User>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.GoogleId).IsUnique();
            _ = entity.HasIndex(e => e.Email).IsUnique();
            _ = entity.Property(e => e.GoogleId).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            _ = entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.ProfilePictureUrl).HasMaxLength(2048);
            _ = entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            _ = entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            _ = entity.HasMany(e => e.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure RefreshToken
        _ = modelBuilder.Entity<RefreshToken>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.HasIndex(e => e.Token).IsUnique();
            _ = entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            _ = entity.Property(e => e.CreatedByIp).HasMaxLength(50);
            _ = entity.Property(e => e.RevokedByIp).HasMaxLength(50);
            _ = entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            _ = entity.Property(e => e.ReasonRevoked).HasMaxLength(256);

            // Add matching query filter for RefreshToken to filter when User is soft deleted
            _ = entity.HasQueryFilter(rt => !rt.User.IsDeleted);
        });
    }

    /// <summary>
    /// Creates a soft delete filter expression for the specified entity type.
    /// </summary>
    private static System.Linq.Expressions.LambdaExpression GetSoftDeleteFilter(Type type)
    {
        ParameterExpression parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
        MemberExpression property = System.Linq.Expressions.Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
        BinaryExpression condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }

    /// <summary>
    /// Saves all changes made in this context to the database with audit information.
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Saves all changes made in this context to the database with audit information.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates audit fields for tracked entities.
    /// </summary>
    private void UpdateAuditFields()
    {
        IEnumerable<EntityEntry<AuditableEntity>> entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (EntityEntry<AuditableEntity> entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = DateTime.UtcNow;
            }
        }
    }
}
