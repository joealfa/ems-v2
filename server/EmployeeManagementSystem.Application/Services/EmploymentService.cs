using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Application.Interfaces;
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
        var employment = await _employmentRepository.Query()
            .Include(e => e.Person)
            .Include(e => e.Position)
            .Include(e => e.SalaryGrade)
            .Include(e => e.Item)
            .Include(e => e.EmploymentSchools.Where(es => !es.IsDeleted))
                .ThenInclude(es => es.School)
            .FirstOrDefaultAsync(e => e.DisplayId == displayId, cancellationToken);

        return employment == null
            ? Result<EmploymentResponseDto>.NotFound("Employment not found.")
            : Result<EmploymentResponseDto>.Success(MapToResponseDto(employment));
    }

    /// <inheritdoc />
    public async Task<PagedResult<EmploymentListDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = _employmentRepository.Query()
            .Include(e => e.Person)
            .Include(e => e.Position)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.ToLower();
            baseQuery = baseQuery.Where(e =>
                (e.DepEdId != null && e.DepEdId.ToLower().Contains(searchTerm)) ||
                e.Person.FirstName.ToLower().Contains(searchTerm) ||
                e.Person.LastName.ToLower().Contains(searchTerm) ||
                e.Position.TitleName.ToLower().Contains(searchTerm));
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var orderedQuery = query.SortDescending
            ? baseQuery.OrderByDescending(e => e.Person.LastName).ThenByDescending(e => e.Person.FirstName)
            : baseQuery.OrderBy(e => e.Person.LastName).ThenBy(e => e.Person.FirstName);

        var items = await orderedQuery
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
        var person = await _personRepository.GetByDisplayIdAsync(dto.PersonDisplayId, cancellationToken);
        if (person == null)
            return Result<EmploymentResponseDto>.BadRequest($"Person with DisplayId {dto.PersonDisplayId} not found.");

        var position = await _positionRepository.GetByDisplayIdAsync(dto.PositionDisplayId, cancellationToken);
        if (position == null)
            return Result<EmploymentResponseDto>.BadRequest($"Position with DisplayId {dto.PositionDisplayId} not found.");

        var salaryGrade = await _salaryGradeRepository.GetByDisplayIdAsync(dto.SalaryGradeDisplayId, cancellationToken);
        if (salaryGrade == null)
            return Result<EmploymentResponseDto>.BadRequest($"SalaryGrade with DisplayId {dto.SalaryGradeDisplayId} not found.");

        var item = await _itemRepository.GetByDisplayIdAsync(dto.ItemDisplayId, cancellationToken);
        if (item == null)
            return Result<EmploymentResponseDto>.BadRequest($"Item with DisplayId {dto.ItemDisplayId} not found.");

        var employment = new Employment
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

        await _employmentRepository.AddAsync(employment, cancellationToken);

        // Add school assignments if provided
        if (dto.Schools != null && dto.Schools.Count > 0)
        {
            foreach (var schoolDto in dto.Schools)
            {
                var school = await _schoolRepository.GetByDisplayIdAsync(schoolDto.SchoolDisplayId, cancellationToken);
                if (school == null)
                    return Result<EmploymentResponseDto>.BadRequest($"School with DisplayId {schoolDto.SchoolDisplayId} not found.");

                var employmentSchool = new EmploymentSchool
                {
                    EmploymentId = employment.Id,
                    SchoolId = school.Id,
                    StartDate = schoolDto.StartDate,
                    EndDate = schoolDto.EndDate,
                    IsCurrent = schoolDto.IsCurrent,
                    CreatedBy = createdBy
                };
                await _employmentSchoolRepository.AddAsync(employmentSchool, cancellationToken);
                employment.EmploymentSchools.Add(employmentSchool);
            }
        }

        // Reload with navigation properties
        employment.Person = person;
        employment.Position = position;
        employment.SalaryGrade = salaryGrade;
        employment.Item = item;

        return Result<EmploymentResponseDto>.Success(MapToResponseDto(employment));
    }

    /// <inheritdoc />
    public async Task<Result<EmploymentResponseDto>> UpdateAsync(long displayId, UpdateEmploymentDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var employment = await _employmentRepository.Query()
            .Include(e => e.Person)
            .Include(e => e.Position)
            .Include(e => e.SalaryGrade)
            .Include(e => e.Item)
            .Include(e => e.EmploymentSchools.Where(es => !es.IsDeleted))
                .ThenInclude(es => es.School)
            .FirstOrDefaultAsync(e => e.DisplayId == displayId, cancellationToken);

        if (employment == null)
            return Result<EmploymentResponseDto>.NotFound("Employment not found.");

        // Resolve foreign key references by display ID
        var position = await _positionRepository.GetByDisplayIdAsync(dto.PositionDisplayId, cancellationToken);
        if (position == null)
            return Result<EmploymentResponseDto>.BadRequest($"Position with DisplayId {dto.PositionDisplayId} not found.");

        var salaryGrade = await _salaryGradeRepository.GetByDisplayIdAsync(dto.SalaryGradeDisplayId, cancellationToken);
        if (salaryGrade == null)
            return Result<EmploymentResponseDto>.BadRequest($"SalaryGrade with DisplayId {dto.SalaryGradeDisplayId} not found.");

        var item = await _itemRepository.GetByDisplayIdAsync(dto.ItemDisplayId, cancellationToken);
        if (item == null)
            return Result<EmploymentResponseDto>.BadRequest($"Item with DisplayId {dto.ItemDisplayId} not found.");

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

        return Result<EmploymentResponseDto>.Success(MapToResponseDto(employment));
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        var employment = await _employmentRepository.Query()
            .Include(e => e.EmploymentSchools.Where(es => !es.IsDeleted))
            .FirstOrDefaultAsync(e => e.DisplayId == displayId, cancellationToken);

        if (employment == null)
            return Result.NotFound("Employment not found.");

        // Cascade soft delete to related employment schools
        foreach (var employmentSchool in employment.EmploymentSchools)
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
        var employment = await _employmentRepository.GetByDisplayIdAsync(employmentDisplayId, cancellationToken);
        if (employment == null)
            return Result<EmploymentSchoolResponseDto>.NotFound("Employment not found.");

        var school = await _schoolRepository.GetByDisplayIdAsync(dto.SchoolDisplayId, cancellationToken);
        if (school == null)
            return Result<EmploymentSchoolResponseDto>.NotFound($"School with DisplayId {dto.SchoolDisplayId} not found.");

        var employmentSchool = new EmploymentSchool
        {
            EmploymentId = employment.Id,
            SchoolId = school.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCurrent = dto.IsCurrent,
            CreatedBy = createdBy
        };

        await _employmentSchoolRepository.AddAsync(employmentSchool, cancellationToken);

        return Result<EmploymentSchoolResponseDto>.Success(new EmploymentSchoolResponseDto
        {
            DisplayId = employmentSchool.DisplayId,
            SchoolDisplayId = school.DisplayId,
            SchoolName = school.SchoolName,
            StartDate = employmentSchool.StartDate,
            EndDate = employmentSchool.EndDate,
            IsCurrent = employmentSchool.IsCurrent,
            IsActive = employmentSchool.IsActive,
            CreatedOn = employmentSchool.CreatedOn,
            CreatedBy = employmentSchool.CreatedBy
        });
    }

    /// <inheritdoc />
    public async Task<Result> RemoveSchoolAssignmentAsync(long employmentSchoolDisplayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        var employmentSchool = await _employmentSchoolRepository.GetByDisplayIdAsync(employmentSchoolDisplayId, cancellationToken);
        if (employmentSchool == null)
            return Result.NotFound("Employment school assignment not found.");

        employmentSchool.ModifiedBy = deletedBy;
        await _employmentSchoolRepository.DeleteAsync(employmentSchool, cancellationToken);
        return Result.Success();
    }

    private static EmploymentResponseDto MapToResponseDto(Employment employment)
    {
        return new EmploymentResponseDto
        {
            DisplayId = employment.DisplayId,
            DepEdId = employment.DepEdId,
            PSIPOPItemNumber = employment.PSIPOPItemNumber,
            TINId = employment.TINId,
            GSISId = employment.GSISId,
            PhilHealthId = employment.PhilHealthId,
            DateOfOriginalAppointment = employment.DateOfOriginalAppointment,
            AppointmentStatus = employment.AppointmentStatus,
            EmploymentStatus = employment.EmploymentStatus,
            Eligibility = employment.Eligibility,
            IsActive = employment.IsActive,
            CreatedOn = employment.CreatedOn,
            CreatedBy = employment.CreatedBy,
            ModifiedOn = employment.ModifiedOn,
            ModifiedBy = employment.ModifiedBy,
            Person = new EmploymentPersonDto
            {
                DisplayId = employment.Person.DisplayId,
                FullName = employment.Person.FullName
            },
            Position = new EmploymentPositionDto
            {
                DisplayId = employment.Position.DisplayId,
                TitleName = employment.Position.TitleName
            },
            SalaryGrade = new EmploymentSalaryGradeDto
            {
                DisplayId = employment.SalaryGrade.DisplayId,
                SalaryGradeName = employment.SalaryGrade.SalaryGradeName,
                Step = employment.SalaryGrade.Step,
                MonthlySalary = employment.SalaryGrade.MonthlySalary
            },
            Item = new EmploymentItemDto
            {
                DisplayId = employment.Item.DisplayId,
                ItemName = employment.Item.ItemName
            },
            Schools = employment.EmploymentSchools.Select(es => new EmploymentSchoolResponseDto
            {
                DisplayId = es.DisplayId,
                SchoolDisplayId = es.School.DisplayId,
                SchoolName = es.School.SchoolName,
                StartDate = es.StartDate,
                EndDate = es.EndDate,
                IsCurrent = es.IsCurrent,
                IsActive = es.IsActive,
                CreatedOn = es.CreatedOn,
                CreatedBy = es.CreatedBy,
                ModifiedOn = es.ModifiedOn,
                ModifiedBy = es.ModifiedBy
            }).ToList()
        };
    }
}
