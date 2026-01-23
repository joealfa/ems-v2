# Database Documentation

This document describes the database configuration, schema, and data access patterns in the EMS backend.

---

## Overview

The application uses **Entity Framework Core** with **SQL Server** for data persistence.

---

## Configuration

### Connection Strings

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeManagementSystem;Trusted_Connection=True;MultipleActiveResultSets=true",
    "BlobStorage": "DefaultEndpointsProtocol=https;AccountName=youraccount;AccountKey=yourkey;EndpointSuffix=core.windows.net"
  }
}
```

### DbContext Registration

**Program.cs:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

## ApplicationDbContext

The main database context class.

**Location:** `EmployeeManagementSystem.Infrastructure/Data/ApplicationDbContext.cs`

### DbSets

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Employment> Employments { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<SalaryGrade> SalaryGrades { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<EmploymentSchool> EmploymentSchools { get; set; }
}
```

### Model Configuration

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Apply all entity configurations
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    
    // Global query filter for soft delete
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
        }
    }
}

private static LambdaExpression CreateSoftDeleteFilter(Type entityType)
{
    var parameter = Expression.Parameter(entityType, "e");
    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
    var condition = Expression.Equal(property, Expression.Constant(false));
    return Expression.Lambda(condition, parameter);
}
```

### Save Changes Override

Automatic audit field updates:

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var entries = ChangeTracker.Entries<BaseEntity>();
    
    foreach (var entry in entries)
    {
        if (entry.State == EntityState.Modified)
        {
            entry.Entity.ModifiedAt = DateTime.UtcNow;
            entry.Entity.ModifiedBy = "System"; // TODO: Get from auth context
        }
    }
    
    return await base.SaveChangesAsync(cancellationToken);
}
```

---

## Entity Configurations

### Person Configuration

```csharp
public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons");
        
        builder.HasKey(p => p.Id);
        
        builder.HasIndex(p => p.DisplayId).IsUnique();
        
        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.MiddleName)
            .HasMaxLength(100);
        
        builder.Property(p => p.ProfileImageUrl)
            .HasMaxLength(2048);
        
        // Relationships
        builder.HasMany(p => p.Addresses)
            .WithOne(a => a.Person)
            .HasForeignKey(a => a.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(p => p.Contacts)
            .WithOne(c => c.Person)
            .HasForeignKey(c => c.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(p => p.Documents)
            .WithOne(d => d.Person)
            .HasForeignKey(d => d.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(p => p.Employments)
            .WithOne(e => e.Person)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### Employment Configuration

```csharp
public class EmploymentConfiguration : IEntityTypeConfiguration<Employment>
{
    public void Configure(EntityTypeBuilder<Employment> builder)
    {
        builder.ToTable("Employments");
        
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.DisplayId).IsUnique();
        
        builder.Property(e => e.DepEdId).HasMaxLength(50);
        builder.Property(e => e.PSIPOPItemNumber).HasMaxLength(50);
        builder.Property(e => e.TINId).HasMaxLength(20);
        builder.Property(e => e.GSISId).HasMaxLength(20);
        builder.Property(e => e.PhilHealthId).HasMaxLength(20);
        
        // Relationships
        builder.HasOne(e => e.Person)
            .WithMany(p => p.Employments)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Position)
            .WithMany(p => p.Employments)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.SalaryGrade)
            .WithMany(sg => sg.Employments)
            .HasForeignKey(e => e.SalaryGradeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Item)
            .WithMany(i => i.Employments)
            .HasForeignKey(e => e.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### EmploymentSchool Configuration (Junction Table)

```csharp
public class EmploymentSchoolConfiguration : IEntityTypeConfiguration<EmploymentSchool>
{
    public void Configure(EntityTypeBuilder<EmploymentSchool> builder)
    {
        builder.ToTable("EmploymentSchools");
        
        builder.HasKey(es => es.Id);
        builder.HasIndex(es => es.DisplayId).IsUnique();
        
        // Composite unique constraint
        builder.HasIndex(es => new { es.EmploymentId, es.SchoolId }).IsUnique();
        
        builder.HasOne(es => es.Employment)
            .WithMany(e => e.EmploymentSchools)
            .HasForeignKey(es => es.EmploymentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(es => es.School)
            .WithMany(s => s.EmploymentSchools)
            .HasForeignKey(es => es.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

---

## Database Schema

### Tables

|        Table        |        Description         |
|---------------------|----------------------------|
| `Persons`           | Individual records         |
| `Employments`       | Employment records         |
| `Schools`           | Educational institutions   |
| `Positions`         | Job positions              |
| `SalaryGrades`      | Salary classifications     |
| `Items`             | Plantilla items            |
| `Documents`         | File metadata              |
| `Addresses`         | Address records            |
| `Contacts`          | Contact information        |
| `EmploymentSchools` | Employment-School junction |

### Common Columns (All Tables)

|    Column    |        Type        |        Description       |
|--------------|--------------------|--------------------------|
| `Id`         | `uniqueidentifier` | Primary key              |
| `DisplayId`  | `bigint`           | Unique public identifier |
| `CreatedBy`  | `nvarchar(100)`    | Creator                  |
| `CreatedAt`  | `datetime2`        | Creation timestamp       |
| `ModifiedBy` | `nvarchar(100)`    | Last modifier            |
| `ModifiedAt` | `datetime2`        | Last modification        |
| `IsDeleted`  | `bit`              | Soft delete flag         |

### Indexes

|        Table      |                   Index                    |         Columns        |    Type    |
|-------------------|--------------------------------------------|------------------------|------------|
| Persons           | IX_Persons_DisplayId                       | DisplayId              | Unique     |
| Employments       | IX_Employments_DisplayId                   | DisplayId              | Unique     |
| Employments       | IX_Employments_PersonId                    | PersonId               | Non-unique |
| EmploymentSchools | IX_EmploymentSchools_EmploymentId_SchoolId | EmploymentId, SchoolId | Unique     |
| Documents         | IX_Documents_PersonId                      | PersonId               | Non-unique |
| Addresses         | IX_Addresses_PersonId                      | PersonId               | Non-unique |
| Addresses         | IX_Addresses_SchoolId                      | SchoolId               | Non-unique |

---

## Migrations

### Migration History

|             Migration            |    Date    |         Description          |
|----------------------------------|------------|------------------------------|
| `InitialCreate`                  | 2026-01-21 | Initial schema               |
| `RemoveDisplayIdIdentity`        | 2026-01-21 | Changed DisplayId generation |
| `AddDocumentsAndProfileImage`    | 2026-01-21 | Added Document entity        |
| `UpdateDeleteBehaviorToRestrict` | 2026-01-21 | Changed delete behavior      |

### Running Migrations

**Apply migrations:**
```bash
dotnet ef database update --project EmployeeManagementSystem.Infrastructure --startup-project EmployeeManagementSystem.Api
```

**Create new migration:**
```bash
dotnet ef migrations add MigrationName --project EmployeeManagementSystem.Infrastructure --startup-project EmployeeManagementSystem.Api
```

**Generate SQL script:**
```bash
dotnet ef migrations script --project EmployeeManagementSystem.Infrastructure --startup-project EmployeeManagementSystem.Api
```

---

## Repository Pattern

### Generic Repository

**Location:** `EmployeeManagementSystem.Infrastructure/Repositories/Repository.cs`

```csharp
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;
    
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public async Task<T?> GetByDisplayIdAsync(long displayId)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.DisplayId == displayId);
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }
    
    public async Task<PagedResult<T>> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = false)
    {
        var query = _dbSet.AsQueryable();
        
        if (filter != null)
            query = query.Where(filter);
        
        var totalCount = await query.CountAsync();
        
        if (orderBy != null)
            query = descending 
                ? query.OrderByDescending(orderBy) 
                : query.OrderBy(orderBy);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
    
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;  // Soft delete
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }
    
    public async Task<bool> ExistsAsync(long displayId)
    {
        return await _dbSet.AnyAsync(e => e.DisplayId == displayId);
    }
    
    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }
}
```

---

## Data Seeding

### Development Seed Data

**Location:** `EmployeeManagementSystem.Infrastructure/Data/SeedData.cs`

```csharp
public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Persons.AnyAsync())
            return;  // Already seeded
        
        // Seed positions
        var positions = new List<Position>
        {
            new Position { TitleName = "Teacher I", Description = "Entry level teacher" },
            new Position { TitleName = "Teacher II", Description = "Second level teacher" },
            new Position { TitleName = "Teacher III", Description = "Third level teacher" },
            new Position { TitleName = "Master Teacher I", Description = "Master teacher" },
            new Position { TitleName = "Principal I", Description = "School principal" }
        };
        context.Positions.AddRange(positions);
        
        // Seed salary grades
        var salaryGrades = new List<SalaryGrade>
        {
            new SalaryGrade { SalaryGradeName = "SG-11", Step = 1, MonthlySalary = 27000m },
            new SalaryGrade { SalaryGradeName = "SG-12", Step = 1, MonthlySalary = 29000m },
            new SalaryGrade { SalaryGradeName = "SG-13", Step = 1, MonthlySalary = 31000m },
            // ... more salary grades
        };
        context.SalaryGrades.AddRange(salaryGrades);
        
        // Seed schools
        var schools = new List<School>
        {
            new School 
            { 
                SchoolName = "Sample Elementary School",
                Addresses = new List<Address>
                {
                    new Address 
                    { 
                        Address1 = "123 Main St",
                        City = "Manila",
                        Province = "Metro Manila",
                        AddressType = AddressType.Business,
                        IsCurrent = true
                    }
                }
            }
        };
        context.Schools.AddRange(schools);
        
        await context.SaveChangesAsync();
    }
}
```

### Running Seed Data

In `Program.cs`:

```csharp
// After app build, before app.Run()
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Apply pending migrations
    await context.Database.MigrateAsync();
    
    // Seed data (development only)
    if (app.Environment.IsDevelopment())
    {
        await SeedData.SeedAsync(context);
    }
}
```

---

## Query Patterns

### Eager Loading

```csharp
var person = await _context.Persons
    .Include(p => p.Addresses)
    .Include(p => p.Contacts)
    .Include(p => p.Documents)
    .FirstOrDefaultAsync(p => p.DisplayId == displayId);
```

### Filtered Include

```csharp
var person = await _context.Persons
    .Include(p => p.Addresses.Where(a => !a.IsDeleted && a.IsActive))
    .Include(p => p.Contacts.Where(c => !c.IsDeleted && c.IsActive))
    .FirstOrDefaultAsync(p => p.DisplayId == displayId);
```

### Projection

```csharp
var personList = await _context.Persons
    .Select(p => new PersonListDto
    {
        DisplayId = p.DisplayId,
        FullName = p.FullName,
        DateOfBirth = p.DateOfBirth,
        Gender = p.Gender
    })
    .ToListAsync();
```

### Pagination with Sorting

```csharp
var query = _context.Persons.AsQueryable();

// Apply search filter
if (!string.IsNullOrEmpty(search))
{
    query = query.Where(p => 
        p.FirstName.Contains(search) || 
        p.LastName.Contains(search));
}

// Apply sorting
query = sortBy switch
{
    "name" => query.OrderBy(p => p.FullName),
    "dateOfBirth" => query.OrderBy(p => p.DateOfBirth),
    _ => query.OrderByDescending(p => p.CreatedAt)
};

// Apply pagination
var items = await query
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```
