using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Data;

/// <summary>
/// The main database context for the Employee Management System.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

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
    /// Configures the model using Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
            }
        }

        // Configure Person
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            entity.HasMany(e => e.Addresses)
                .WithOne(a => a.Person)
                .HasForeignKey(a => a.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Contacts)
                .WithOne(c => c.Person)
                .HasForeignKey(c => c.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Employments)
                .WithOne(emp => emp.Person)
                .HasForeignKey(emp => emp.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Documents)
                .WithOne(d => d.Person)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.ProfileImageUrl).HasMaxLength(2048);
        });

        // Configure Address
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.Address1).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address2).HasMaxLength(200);
            entity.Property(e => e.Barangay).HasMaxLength(100);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Province).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure Contact
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.Mobile).HasMaxLength(20);
            entity.Property(e => e.LandLine).HasMaxLength(20);
            entity.Property(e => e.Fax).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure School
        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.SchoolName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            entity.HasMany(e => e.Addresses)
                .WithOne(a => a.School)
                .HasForeignKey(a => a.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Contacts)
                .WithOne(c => c.School)
                .HasForeignKey(c => c.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.ItemName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure Position
        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.TitleName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure SalaryGrade
        modelBuilder.Entity<SalaryGrade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.SalaryGradeName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MonthlySalary).HasPrecision(18, 2);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });

        // Configure Employment
        modelBuilder.Entity<Employment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.DepEdId).HasMaxLength(50);
            entity.Property(e => e.PSIPOPItemNumber).HasMaxLength(50);
            entity.Property(e => e.TINId).HasMaxLength(20);
            entity.Property(e => e.GSISId).HasMaxLength(20);
            entity.Property(e => e.PhilHealthId).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            entity.HasOne(e => e.Position)
                .WithMany(p => p.Employments)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SalaryGrade)
                .WithMany(sg => sg.Employments)
                .HasForeignKey(e => e.SalaryGradeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Item)
                .WithMany(i => i.Employments)
                .HasForeignKey(e => e.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure EmploymentSchool (Many-to-Many)
        modelBuilder.Entity<EmploymentSchool>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);

            entity.HasIndex(e => new { e.EmploymentId, e.SchoolId, e.StartDate }).IsUnique();

            entity.HasOne(es => es.Employment)
                .WithMany(e => e.EmploymentSchools)
                .HasForeignKey(es => es.EmploymentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(es => es.School)
                .WithMany(s => s.EmploymentSchools)
                .HasForeignKey(es => es.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Document
        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DisplayId).IsUnique();
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileExtension).IsRequired().HasMaxLength(10);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BlobUrl).IsRequired().HasMaxLength(2048);
            entity.Property(e => e.BlobName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContainerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ModifiedBy).HasMaxLength(256);
        });
    }

    /// <summary>
    /// Creates a soft delete filter expression for the specified entity type.
    /// </summary>
    private static System.Linq.Expressions.LambdaExpression GetSoftDeleteFilter(Type type)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(AuditableEntity.IsDeleted));
        var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
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
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedOn = DateTime.UtcNow;
            }
        }
    }
}
