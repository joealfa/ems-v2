using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.School;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Mappings;
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
        School? school = await _schoolRepository.Query()
            .Include(s => s.Addresses.Where(a => !a.IsDeleted))
            .Include(s => s.Contacts.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(s => s.DisplayId == displayId, cancellationToken);

        return school == null
            ? Result<SchoolResponseDto>.NotFound("School not found.")
            : Result<SchoolResponseDto>.Success(school.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<PagedResult<SchoolListDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<School> queryable = _schoolRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            string searchTerm = query.SearchTerm.ToLower();
            queryable = queryable.Where(s => s.SchoolName.ToLower().Contains(searchTerm));
        }

        int totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(s => s.SchoolName)
            : queryable.OrderBy(s => s.SchoolName);

        List<SchoolListDto> items = await queryable
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
        School school = new()
        {
            SchoolName = dto.SchoolName,
            CreatedBy = createdBy
        };

        _ = await _schoolRepository.AddAsync(school, cancellationToken);

        // Add addresses if provided
        if (dto.Addresses != null && dto.Addresses.Count > 0)
        {
            foreach (CreateAddressDto addressDto in dto.Addresses)
            {
                Address address = new()
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
                _ = await _addressRepository.AddAsync(address, cancellationToken);
                school.Addresses.Add(address);
            }
        }

        // Add contacts if provided
        if (dto.Contacts != null && dto.Contacts.Count > 0)
        {
            foreach (CreateContactDto contactDto in dto.Contacts)
            {
                Contact contact = new()
                {
                    Mobile = contactDto.Mobile,
                    LandLine = contactDto.LandLine,
                    Fax = contactDto.Fax,
                    Email = contactDto.Email,
                    ContactType = contactDto.ContactType,
                    SchoolId = school.Id,
                    CreatedBy = createdBy
                };
                _ = await _contactRepository.AddAsync(contact, cancellationToken);
                school.Contacts.Add(contact);
            }
        }

        return Result<SchoolResponseDto>.Success(school.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result<SchoolResponseDto>> UpdateAsync(long displayId, UpdateSchoolDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        School? school = await _schoolRepository.Query()
            .Include(s => s.Addresses.Where(a => !a.IsDeleted))
            .Include(s => s.Contacts.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(s => s.DisplayId == displayId, cancellationToken);

        if (school == null)
        {
            return Result<SchoolResponseDto>.NotFound("School not found.");
        }

        school.SchoolName = dto.SchoolName;
        school.IsActive = dto.IsActive;
        school.ModifiedBy = modifiedBy;

        // Sync addresses if provided
        if (dto.Addresses != null)
        {
            HashSet<long> addressDisplayIds = dto.Addresses
                .Where(a => a.DisplayId.HasValue)
                .Select(a => a.DisplayId!.Value)
                .ToHashSet();

            // Soft-delete addresses not in the list
            foreach (Address existing in school.Addresses.Where(a => !addressDisplayIds.Contains(a.DisplayId)).ToList())
            {
                existing.ModifiedBy = modifiedBy;
                await _addressRepository.DeleteAsync(existing, cancellationToken);
                _ = school.Addresses.Remove(existing);
            }

            // Update existing / create new
            foreach (UpsertAddressDto addrDto in dto.Addresses)
            {
                if (addrDto.DisplayId.HasValue)
                {
                    // Update existing
                    Address? existing = school.Addresses.FirstOrDefault(a => a.DisplayId == addrDto.DisplayId);
                    if (existing != null)
                    {
                        existing.Address1 = addrDto.Address1;
                        existing.Address2 = addrDto.Address2;
                        existing.Barangay = addrDto.Barangay;
                        existing.City = addrDto.City;
                        existing.Province = addrDto.Province;
                        existing.Country = addrDto.Country;
                        existing.ZipCode = addrDto.ZipCode;
                        existing.IsCurrent = addrDto.IsCurrent;
                        existing.IsPermanent = addrDto.IsPermanent;
                        existing.AddressType = addrDto.AddressType;
                        existing.ModifiedBy = modifiedBy;
                        await _addressRepository.UpdateAsync(existing, cancellationToken);
                    }
                }
                else
                {
                    // Create new
                    Address newAddress = new()
                    {
                        Address1 = addrDto.Address1,
                        Address2 = addrDto.Address2,
                        Barangay = addrDto.Barangay,
                        City = addrDto.City,
                        Province = addrDto.Province,
                        Country = addrDto.Country,
                        ZipCode = addrDto.ZipCode,
                        IsCurrent = addrDto.IsCurrent,
                        IsPermanent = addrDto.IsPermanent,
                        AddressType = addrDto.AddressType,
                        SchoolId = school.Id,
                        CreatedBy = modifiedBy
                    };
                    _ = await _addressRepository.AddAsync(newAddress, cancellationToken);
                    school.Addresses.Add(newAddress);
                }
            }
        }

        // Sync contacts if provided
        if (dto.Contacts != null)
        {
            HashSet<long> contactDisplayIds = dto.Contacts
                .Where(c => c.DisplayId.HasValue)
                .Select(c => c.DisplayId!.Value)
                .ToHashSet();

            // Soft-delete contacts not in the list
            foreach (Contact existing in school.Contacts.Where(c => !contactDisplayIds.Contains(c.DisplayId)).ToList())
            {
                existing.ModifiedBy = modifiedBy;
                await _contactRepository.DeleteAsync(existing, cancellationToken);
                _ = school.Contacts.Remove(existing);
            }

            // Update existing / create new
            foreach (UpsertContactDto contactDto in dto.Contacts)
            {
                if (contactDto.DisplayId.HasValue)
                {
                    // Update existing
                    Contact? existing = school.Contacts.FirstOrDefault(c => c.DisplayId == contactDto.DisplayId);
                    if (existing != null)
                    {
                        existing.Mobile = contactDto.Mobile;
                        existing.LandLine = contactDto.LandLine;
                        existing.Fax = contactDto.Fax;
                        existing.Email = contactDto.Email;
                        existing.ContactType = contactDto.ContactType;
                        existing.ModifiedBy = modifiedBy;
                        await _contactRepository.UpdateAsync(existing, cancellationToken);
                    }
                }
                else
                {
                    // Create new
                    Contact newContact = new()
                    {
                        Mobile = contactDto.Mobile,
                        LandLine = contactDto.LandLine,
                        Fax = contactDto.Fax,
                        Email = contactDto.Email,
                        ContactType = contactDto.ContactType,
                        SchoolId = school.Id,
                        CreatedBy = modifiedBy
                    };
                    _ = await _contactRepository.AddAsync(newContact, cancellationToken);
                    school.Contacts.Add(newContact);
                }
            }
        }

        await _schoolRepository.UpdateAsync(school, cancellationToken);

        return Result<SchoolResponseDto>.Success(school.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        School? school = await _schoolRepository.Query()
            .Include(s => s.Addresses.Where(a => !a.IsDeleted))
            .Include(s => s.Contacts.Where(c => !c.IsDeleted))
            .Include(s => s.EmploymentSchools.Where(es => !es.IsDeleted))
            .FirstOrDefaultAsync(s => s.DisplayId == displayId, cancellationToken);

        if (school == null)
        {
            return Result.NotFound("School not found.");
        }

        // Cascade soft delete to related addresses
        foreach (Address address in school.Addresses)
        {
            address.ModifiedBy = deletedBy;
            await _addressRepository.DeleteAsync(address, cancellationToken);
        }

        // Cascade soft delete to related contacts
        foreach (Contact contact in school.Contacts)
        {
            contact.ModifiedBy = deletedBy;
            await _contactRepository.DeleteAsync(contact, cancellationToken);
        }

        // Cascade soft delete to related employment schools
        foreach (EmploymentSchool employmentSchool in school.EmploymentSchools)
        {
            employmentSchool.ModifiedBy = deletedBy;
            await _employmentSchoolRepository.DeleteAsync(employmentSchool, cancellationToken);
        }

        // Soft delete the school
        school.ModifiedBy = deletedBy;
        await _schoolRepository.DeleteAsync(school, cancellationToken);
        return Result.Success();
    }
}
