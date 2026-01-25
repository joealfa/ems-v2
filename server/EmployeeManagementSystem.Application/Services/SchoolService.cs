using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.School;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for school operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SchoolService"/> class.
/// </remarks>
public class SchoolService(
    IRepository<School> schoolRepository,
    IRepository<Address> addressRepository,
    IRepository<Contact> contactRepository,
    IRepository<EmploymentSchool> employmentSchoolRepository) : ISchoolService
{
    private readonly IRepository<School> _schoolRepository = schoolRepository;
    private readonly IRepository<Address> _addressRepository = addressRepository;
    private readonly IRepository<Contact> _contactRepository = contactRepository;
    private readonly IRepository<EmploymentSchool> _employmentSchoolRepository = employmentSchoolRepository;

    /// <inheritdoc />
    public async Task<Result<SchoolResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        var school = await _schoolRepository.Query()
            .Include(s => s.Addresses.Where(a => !a.IsDeleted))
            .Include(s => s.Contacts.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(s => s.DisplayId == displayId, cancellationToken);

        return school == null ? Result<SchoolResponseDto>.NotFound("School not found.") : Result<SchoolResponseDto>.Success(MapToResponseDto(school));
    }

    /// <inheritdoc />
    public async Task<PagedResult<SchoolListDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _schoolRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.ToLower();
            queryable = queryable.Where(s => s.SchoolName.ToLower().Contains(searchTerm));
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(s => s.SchoolName)
            : queryable.OrderBy(s => s.SchoolName);

        var items = await queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new SchoolListDto
            {
                DisplayId = s.DisplayId,
                SchoolName = s.SchoolName,
                IsActive = s.IsActive,
                CreatedOn = s.CreatedOn,
                CreatedBy = s.CreatedBy,
                ModifiedOn = s.ModifiedOn,
                ModifiedBy = s.ModifiedBy
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SchoolListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<Result<SchoolResponseDto>> CreateAsync(CreateSchoolDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var school = new School
        {
            SchoolName = dto.SchoolName,
            CreatedBy = createdBy
        };

        await _schoolRepository.AddAsync(school, cancellationToken);

        // Add addresses if provided
        if (dto.Addresses != null && dto.Addresses.Count > 0)
        {
            foreach (var addressDto in dto.Addresses)
            {
                var address = new Address
                {
                    Address1 = addressDto.Address1,
                    Address2 = addressDto.Address2,
                    Barangay = addressDto.Barangay,
                    City = addressDto.City,
                    Province = addressDto.Province,
                    Country = addressDto.Country,
                    ZipCode = addressDto.ZipCode,
                    IsCurrent = addressDto.IsCurrent,
                    IsPermanent = addressDto.IsPermanent,
                    AddressType = addressDto.AddressType,
                    SchoolId = school.Id,
                    CreatedBy = createdBy
                };
                await _addressRepository.AddAsync(address, cancellationToken);
                school.Addresses.Add(address);
            }
        }

        // Add contacts if provided
        if (dto.Contacts != null && dto.Contacts.Count > 0)
        {
            foreach (var contactDto in dto.Contacts)
            {
                var contact = new Contact
                {
                    Mobile = contactDto.Mobile,
                    LandLine = contactDto.LandLine,
                    Fax = contactDto.Fax,
                    Email = contactDto.Email,
                    ContactType = contactDto.ContactType,
                    SchoolId = school.Id,
                    CreatedBy = createdBy
                };
                await _contactRepository.AddAsync(contact, cancellationToken);
                school.Contacts.Add(contact);
            }
        }

        return Result<SchoolResponseDto>.Success(MapToResponseDto(school));
    }

    /// <inheritdoc />
    public async Task<Result<SchoolResponseDto>> UpdateAsync(long displayId, UpdateSchoolDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var school = await _schoolRepository.Query()
            .Include(s => s.Addresses.Where(a => !a.IsDeleted))
            .Include(s => s.Contacts.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(s => s.DisplayId == displayId, cancellationToken);

        if (school == null)
            return Result<SchoolResponseDto>.NotFound("School not found.");

        school.SchoolName = dto.SchoolName;
        school.IsActive = dto.IsActive;
        school.ModifiedBy = modifiedBy;

        await _schoolRepository.UpdateAsync(school, cancellationToken);

        return Result<SchoolResponseDto>.Success(MapToResponseDto(school));
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        var school = await _schoolRepository.Query()
            .Include(s => s.Addresses.Where(a => !a.IsDeleted))
            .Include(s => s.Contacts.Where(c => !c.IsDeleted))
            .Include(s => s.EmploymentSchools.Where(es => !es.IsDeleted))
            .FirstOrDefaultAsync(s => s.DisplayId == displayId, cancellationToken);

        if (school == null)
            return Result.NotFound("School not found.");

        // Cascade soft delete to related addresses
        foreach (var address in school.Addresses)
        {
            address.ModifiedBy = deletedBy;
            await _addressRepository.DeleteAsync(address, cancellationToken);
        }

        // Cascade soft delete to related contacts
        foreach (var contact in school.Contacts)
        {
            contact.ModifiedBy = deletedBy;
            await _contactRepository.DeleteAsync(contact, cancellationToken);
        }

        // Cascade soft delete to related employment schools
        foreach (var employmentSchool in school.EmploymentSchools)
        {
            employmentSchool.ModifiedBy = deletedBy;
            await _employmentSchoolRepository.DeleteAsync(employmentSchool, cancellationToken);
        }

        // Soft delete the school
        school.ModifiedBy = deletedBy;
        await _schoolRepository.DeleteAsync(school, cancellationToken);
        return Result.Success();
    }

    private static SchoolResponseDto MapToResponseDto(School school)
    {
        return new SchoolResponseDto
        {
            DisplayId = school.DisplayId,
            SchoolName = school.SchoolName,
            IsActive = school.IsActive,
            CreatedOn = school.CreatedOn,
            CreatedBy = school.CreatedBy,
            ModifiedOn = school.ModifiedOn,
            ModifiedBy = school.ModifiedBy,
            Addresses = school.Addresses.Select(a => new AddressResponseDto
            {
                DisplayId = a.DisplayId,
                Address1 = a.Address1,
                Address2 = a.Address2,
                Barangay = a.Barangay,
                City = a.City,
                Province = a.Province,
                Country = a.Country,
                ZipCode = a.ZipCode,
                IsCurrent = a.IsCurrent,
                IsPermanent = a.IsPermanent,
                IsActive = a.IsActive,
                AddressType = a.AddressType,
                FullAddress = a.FullAddress,
                CreatedOn = a.CreatedOn,
                CreatedBy = a.CreatedBy,
                ModifiedOn = a.ModifiedOn,
                ModifiedBy = a.ModifiedBy
            }).ToList(),
            Contacts = school.Contacts.Select(c => new ContactResponseDto
            {
                DisplayId = c.DisplayId,
                Mobile = c.Mobile,
                LandLine = c.LandLine,
                Fax = c.Fax,
                Email = c.Email,
                IsActive = c.IsActive,
                ContactType = c.ContactType,
                CreatedOn = c.CreatedOn,
                CreatedBy = c.CreatedBy,
                ModifiedOn = c.ModifiedOn,
                ModifiedBy = c.ModifiedBy
            }).ToList()
        };
    }
}
