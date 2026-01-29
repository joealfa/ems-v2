using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Mappings;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for employment operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmploymentService"/> class.
/// </remarks>
public class EmploymentService(
    IRepository<Employment> employmentRepository,
    IRepository<EmploymentSchool> employmentSchoolRepository,
    IRepository<Person> personRepository,
    IRepository<Position> positionRepository,
    IRepository<SalaryGrade> salaryGradeRepository,
    IRepository<Item> itemRepository,
    IRepository<School> schoolRepository) : IEmploymentService
{
    private readonly IRepository<Employment> _employmentRepository = employmentRepository;
    private readonly IRepository<EmploymentSchool> _employmentSchoolRepository = employmentSchoolRepository;
    private readonly IRepository<Person> _personRepository = personRepository;
    private readonly IRepository<Position> _positionRepository = positionRepository;
    private readonly IRepository<SalaryGrade> _salaryGradeRepository = salaryGradeRepository;
    private readonly IRepository<Item> _itemRepository = itemRepository;
    private readonly IRepository<School> _schoolRepository = schoolRepository;

    /// <inheritdoc />
    public async Task<Result<EmploymentResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        Employment? employment = await _employmentRepository.Query()
            .Include(e => e.Person)
            .Include(e => e.Position)
            .Include(e => e.SalaryGrade)
            .Include(e => e.Item)
            .Include(e => e.EmploymentSchools.Where(es => !es.IsDeleted))
                .ThenInclude(es => es.School)
            .FirstOrDefaultAsync(e => e.DisplayId == displayId, cancellationToken);

        return employment == null
            ? Result<EmploymentResponseDto>.NotFound("Employment not found.")
            : Result<EmploymentResponseDto>.Success(employment.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<PagedResult<EmploymentListDto>> GetPagedAsync(EmploymentPaginationQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Employment> baseQuery = _employmentRepository.Query()
            .Include(e => e.Person)
            .Include(e => e.Position)
            .AsQueryable();

        // Apply search term filter (searches across name fields and DepEd ID)
        // Split by spaces to handle multi-word searches like "John Doe"
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            string[] searchTerms = query.SearchTerm.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string term in searchTerms)
            {
                baseQuery = baseQuery.Where(e =>
                    (e.DepEdId != null && e.DepEdId.ToLower().Contains(term)) ||
                    e.Person.FirstName.ToLower().Contains(term) ||
                    e.Person.LastName.ToLower().Contains(term) ||
                    (e.Person.MiddleName != null && e.Person.MiddleName.ToLower().Contains(term)) ||
                    e.Position.TitleName.ToLower().Contains(term));
            }
        }

        // Apply column-specific filters
        if (!string.IsNullOrWhiteSpace(query.DisplayIdFilter))
        {
            baseQuery = baseQuery.Where(e => e.DisplayId.ToString().Contains(query.DisplayIdFilter));
        }

        // Split employee name filter by spaces to handle multi-word searches
        if (!string.IsNullOrWhiteSpace(query.EmployeeNameFilter))
        {
            string[] filterTerms = query.EmployeeNameFilter.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string term in filterTerms)
            {
                baseQuery = baseQuery.Where(e =>
                    e.Person.FirstName.ToLower().Contains(term) ||
                    e.Person.LastName.ToLower().Contains(term) ||
                    (e.Person.MiddleName != null && e.Person.MiddleName.ToLower().Contains(term)));
            }
        }

        if (!string.IsNullOrWhiteSpace(query.DepEdIdFilter))
        {
            string depEdIdFilter = query.DepEdIdFilter.ToLower();
            baseQuery = baseQuery.Where(e => e.DepEdId != null && e.DepEdId.ToLower().Contains(depEdIdFilter));
        }

        if (!string.IsNullOrWhiteSpace(query.PositionFilter))
        {
            string positionFilter = query.PositionFilter.ToLower();
            baseQuery = baseQuery.Where(e => e.Position.TitleName.ToLower().Contains(positionFilter));
        }

        if (query.EmploymentStatus.HasValue)
        {
            baseQuery = baseQuery.Where(e => e.EmploymentStatus == query.EmploymentStatus.Value);
        }

        if (query.IsActive.HasValue)
        {
            baseQuery = baseQuery.Where(e => e.IsActive == query.IsActive.Value);
        }

        int totalCount = await baseQuery.CountAsync(cancellationToken);

        IOrderedQueryable<Employment> orderedQuery = query.SortDescending
            ? baseQuery.OrderByDescending(e => e.Person.LastName).ThenByDescending(e => e.Person.FirstName)
            : baseQuery.OrderBy(e => e.Person.LastName).ThenBy(e => e.Person.FirstName);

        List<EmploymentListDto> items = await orderedQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(e => new EmploymentListDto
            {
                DisplayId = e.DisplayId,
                DepEdId = e.DepEdId,
                EmployeeFullName = e.Person.FullName,
                PositionTitle = e.Position.TitleName,
                EmploymentStatus = e.EmploymentStatus,
                IsActive = e.IsActive,
                CreatedOn = e.CreatedOn,
                CreatedBy = e.CreatedBy,
                ModifiedOn = e.ModifiedOn,
                ModifiedBy = e.ModifiedBy
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<EmploymentListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<Result<EmploymentResponseDto>> CreateAsync(CreateEmploymentDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        // Resolve foreign key references by display ID
        Person? person = await _personRepository.GetByDisplayIdAsync(dto.PersonDisplayId, cancellationToken);
        if (person == null)
        {
            return Result<EmploymentResponseDto>.BadRequest($"Person with DisplayId {dto.PersonDisplayId} not found.");
        }

        Position? position = await _positionRepository.GetByDisplayIdAsync(dto.PositionDisplayId, cancellationToken);
        if (position == null)
        {
            return Result<EmploymentResponseDto>.BadRequest($"Position with DisplayId {dto.PositionDisplayId} not found.");
        }

        SalaryGrade? salaryGrade = await _salaryGradeRepository.GetByDisplayIdAsync(dto.SalaryGradeDisplayId, cancellationToken);
        if (salaryGrade == null)
        {
            return Result<EmploymentResponseDto>.BadRequest($"SalaryGrade with DisplayId {dto.SalaryGradeDisplayId} not found.");
        }

        Item? item = await _itemRepository.GetByDisplayIdAsync(dto.ItemDisplayId, cancellationToken);
        if (item == null)
        {
            return Result<EmploymentResponseDto>.BadRequest($"Item with DisplayId {dto.ItemDisplayId} not found.");
        }

        Employment employment = new()
        {
            DepEdId = dto.DepEdId,
            PSIPOPItemNumber = dto.PSIPOPItemNumber,
            TINId = dto.TINId,
            GSISId = dto.GSISId,
            PhilHealthId = dto.PhilHealthId,
            DateOfOriginalAppointment = dto.DateOfOriginalAppointment,
            AppointmentStatus = dto.AppointmentStatus,
            EmploymentStatus = dto.EmploymentStatus,
            Eligibility = dto.Eligibility,
            PersonId = person.Id,
            PositionId = position.Id,
            SalaryGradeId = salaryGrade.Id,
            ItemId = item.Id,
            CreatedBy = createdBy
        };

        _ = await _employmentRepository.AddAsync(employment, cancellationToken);

        // Add school assignments if provided
        if (dto.Schools != null && dto.Schools.Count > 0)
        {
            foreach (CreateEmploymentSchoolDto schoolDto in dto.Schools)
            {
                School? school = await _schoolRepository.GetByDisplayIdAsync(schoolDto.SchoolDisplayId, cancellationToken);
                if (school == null)
                {
                    return Result<EmploymentResponseDto>.BadRequest($"School with DisplayId {schoolDto.SchoolDisplayId} not found.");
                }

                EmploymentSchool employmentSchool = new()
                {
                    EmploymentId = employment.Id,
                    SchoolId = school.Id,
                    StartDate = schoolDto.StartDate,
                    EndDate = schoolDto.EndDate,
                    IsCurrent = schoolDto.IsCurrent,
                    CreatedBy = createdBy,
                    School = school
                };
                _ = await _employmentSchoolRepository.AddAsync(employmentSchool, cancellationToken);
                employment.EmploymentSchools.Add(employmentSchool);
            }
        }

        // Set navigation properties for response
        employment.Person = person;
        employment.Position = position;
        employment.SalaryGrade = salaryGrade;
        employment.Item = item;

        return Result<EmploymentResponseDto>.Success(employment.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result<EmploymentResponseDto>> UpdateAsync(long displayId, UpdateEmploymentDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        Employment? employment = await _employmentRepository.Query()
            .Include(e => e.Person)
            .Include(e => e.Position)
            .Include(e => e.SalaryGrade)
            .Include(e => e.Item)
            .Include(e => e.EmploymentSchools.Where(es => !es.IsDeleted))
                .ThenInclude(es => es.School)
            .FirstOrDefaultAsync(e => e.DisplayId == displayId, cancellationToken);

        if (employment == null)
        {
            return Result<EmploymentResponseDto>.NotFound("Employment not found.");
        }

        // Resolve foreign key references by display ID
        Position? position = await _positionRepository.GetByDisplayIdAsync(dto.PositionDisplayId, cancellationToken);
        if (position == null)
        {
            return Result<EmploymentResponseDto>.BadRequest($"Position with DisplayId {dto.PositionDisplayId} not found.");
        }

        SalaryGrade? salaryGrade = await _salaryGradeRepository.GetByDisplayIdAsync(dto.SalaryGradeDisplayId, cancellationToken);
        if (salaryGrade == null)
        {
            return Result<EmploymentResponseDto>.BadRequest($"SalaryGrade with DisplayId {dto.SalaryGradeDisplayId} not found.");
        }

        Item? item = await _itemRepository.GetByDisplayIdAsync(dto.ItemDisplayId, cancellationToken);
        if (item == null)
        {
            return Result<EmploymentResponseDto>.BadRequest($"Item with DisplayId {dto.ItemDisplayId} not found.");
        }

        employment.DepEdId = dto.DepEdId;
        employment.PSIPOPItemNumber = dto.PSIPOPItemNumber;
        employment.TINId = dto.TINId;
        employment.GSISId = dto.GSISId;
        employment.PhilHealthId = dto.PhilHealthId;
        employment.DateOfOriginalAppointment = dto.DateOfOriginalAppointment;
        employment.AppointmentStatus = dto.AppointmentStatus;
        employment.EmploymentStatus = dto.EmploymentStatus;
        employment.Eligibility = dto.Eligibility;
        employment.PositionId = position.Id;
        employment.SalaryGradeId = salaryGrade.Id;
        employment.ItemId = item.Id;
        employment.IsActive = dto.IsActive;
        employment.ModifiedBy = modifiedBy;

        await _employmentRepository.UpdateAsync(employment, cancellationToken);

        // Update navigation properties for response
        employment.Position = position;
        employment.SalaryGrade = salaryGrade;
        employment.Item = item;

        return Result<EmploymentResponseDto>.Success(employment.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        Employment? employment = await _employmentRepository.Query()
            .Include(e => e.EmploymentSchools.Where(es => !es.IsDeleted))
            .FirstOrDefaultAsync(e => e.DisplayId == displayId, cancellationToken);

        if (employment == null)
        {
            return Result.NotFound("Employment not found.");
        }

        // Cascade soft delete to related employment schools
        foreach (EmploymentSchool employmentSchool in employment.EmploymentSchools)
        {
            employmentSchool.ModifiedBy = deletedBy;
            await _employmentSchoolRepository.DeleteAsync(employmentSchool, cancellationToken);
        }

        // Soft delete the employment
        employment.ModifiedBy = deletedBy;
        await _employmentRepository.DeleteAsync(employment, cancellationToken);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<EmploymentSchoolResponseDto>> AddSchoolAssignmentAsync(long employmentDisplayId, CreateEmploymentSchoolDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        Employment? employment = await _employmentRepository.GetByDisplayIdAsync(employmentDisplayId, cancellationToken);
        if (employment == null)
        {
            return Result<EmploymentSchoolResponseDto>.NotFound("Employment not found.");
        }

        School? school = await _schoolRepository.GetByDisplayIdAsync(dto.SchoolDisplayId, cancellationToken);
        if (school == null)
        {
            return Result<EmploymentSchoolResponseDto>.NotFound($"School with DisplayId {dto.SchoolDisplayId} not found.");
        }

        EmploymentSchool employmentSchool = new()
        {
            EmploymentId = employment.Id,
            SchoolId = school.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCurrent = dto.IsCurrent,
            CreatedBy = createdBy
        };

        _ = await _employmentSchoolRepository.AddAsync(employmentSchool, cancellationToken);

        // Set school for mapping
        employmentSchool.School = school;

        return Result<EmploymentSchoolResponseDto>.Success(employmentSchool.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result> RemoveSchoolAssignmentAsync(long employmentSchoolDisplayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        EmploymentSchool? employmentSchool = await _employmentSchoolRepository.GetByDisplayIdAsync(employmentSchoolDisplayId, cancellationToken);
        if (employmentSchool == null)
        {
            return Result.NotFound("Employment school assignment not found.");
        }

        employmentSchool.ModifiedBy = deletedBy;
        await _employmentSchoolRepository.DeleteAsync(employmentSchool, cancellationToken);
        return Result.Success();
    }
}
