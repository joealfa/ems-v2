using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Person;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for person operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PersonService"/> class.
/// </remarks>
public class PersonService(
    IRepository<Person> personRepository,
    IRepository<Address> addressRepository,
    IRepository<Contact> contactRepository,
    IRepository<Document> documentRepository) : IPersonService
{
    private readonly IRepository<Person> _personRepository = personRepository;
    private readonly IRepository<Address> _addressRepository = addressRepository;
    private readonly IRepository<Contact> _contactRepository = contactRepository;
    private readonly IRepository<Document> _documentRepository = documentRepository;

    /// <inheritdoc />
    public async Task<Result<PersonResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .Include(p => p.Addresses.Where(a => !a.IsDeleted))
            .Include(p => p.Contacts.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(p => p.DisplayId == displayId, cancellationToken);

        return person == null ? Result<PersonResponseDto>.NotFound("Person not found.") : Result<PersonResponseDto>.Success(MapToResponseDto(person));
    }

    /// <inheritdoc />
    public async Task<PagedResult<PersonListDto>> GetPagedAsync(PersonPaginationQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _personRepository.Query();

        // Apply search term filter (searches across name fields)
        // Split by spaces to handle multi-word searches like "John Doe"
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerms = query.SearchTerm.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in searchTerms)
            {
                queryable = queryable.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term) ||
                    (p.MiddleName != null && p.MiddleName.ToLower().Contains(term)));
            }
        }

        // Apply column-specific filters
        if (!string.IsNullOrWhiteSpace(query.DisplayIdFilter))
        {
            queryable = queryable.Where(p => p.DisplayId.ToString().Contains(query.DisplayIdFilter));
        }

        // Split full name filter by spaces to handle multi-word searches
        if (!string.IsNullOrWhiteSpace(query.FullNameFilter))
        {
            var filterTerms = query.FullNameFilter.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in filterTerms)
            {
                queryable = queryable.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term) ||
                    (p.MiddleName != null && p.MiddleName.ToLower().Contains(term)));
            }
        }

        if (query.Gender.HasValue)
        {
            queryable = queryable.Where(p => p.Gender == query.Gender.Value);
        }

        if (query.CivilStatus.HasValue)
        {
            queryable = queryable.Where(p => p.CivilStatus == query.CivilStatus.Value);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(p => p.LastName).ThenByDescending(p => p.FirstName)
            : queryable.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

        var items = await queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new PersonListDto
            {
                DisplayId = p.DisplayId,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                CivilStatus = p.CivilStatus,
                ProfileImageUrl = p.ProfileImageUrl,
                CreatedOn = p.CreatedOn,
                CreatedBy = p.CreatedBy,
                ModifiedOn = p.ModifiedOn,
                ModifiedBy = p.ModifiedBy
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PersonListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<Result<PersonResponseDto>> CreateAsync(CreatePersonDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var person = new Person
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            CivilStatus = dto.CivilStatus,
            CreatedBy = createdBy
        };

        await _personRepository.AddAsync(person, cancellationToken);

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
                    PersonId = person.Id,
                    CreatedBy = createdBy
                };
                await _addressRepository.AddAsync(address, cancellationToken);
                person.Addresses.Add(address);
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
                    PersonId = person.Id,
                    CreatedBy = createdBy
                };
                await _contactRepository.AddAsync(contact, cancellationToken);
                person.Contacts.Add(contact);
            }
        }

        return Result<PersonResponseDto>.Success(MapToResponseDto(person));
    }

    /// <inheritdoc />
    public async Task<Result<PersonResponseDto>> UpdateAsync(long displayId, UpdatePersonDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .Include(p => p.Addresses.Where(a => !a.IsDeleted))
            .Include(p => p.Contacts.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(p => p.DisplayId == displayId, cancellationToken);

        if (person == null)
            return Result<PersonResponseDto>.NotFound("Person not found.");

        person.FirstName = dto.FirstName;
        person.LastName = dto.LastName;
        person.MiddleName = dto.MiddleName;
        person.DateOfBirth = dto.DateOfBirth;
        person.Gender = dto.Gender;
        person.CivilStatus = dto.CivilStatus;
        person.ModifiedBy = modifiedBy;

        await _personRepository.UpdateAsync(person, cancellationToken);

        return Result<PersonResponseDto>.Success(MapToResponseDto(person));
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .Include(p => p.Addresses.Where(a => !a.IsDeleted))
            .Include(p => p.Contacts.Where(c => !c.IsDeleted))
            .Include(p => p.Documents.Where(d => !d.IsDeleted))
            .FirstOrDefaultAsync(p => p.DisplayId == displayId, cancellationToken);

        if (person == null)
            return Result.NotFound("Person not found.");

        // Cascade soft delete to related addresses
        foreach (var address in person.Addresses)
        {
            address.ModifiedBy = deletedBy;
            await _addressRepository.DeleteAsync(address, cancellationToken);
        }

        // Cascade soft delete to related contacts
        foreach (var contact in person.Contacts)
        {
            contact.ModifiedBy = deletedBy;
            await _contactRepository.DeleteAsync(contact, cancellationToken);
        }

        // Cascade soft delete to related documents
        foreach (var document in person.Documents)
        {
            document.ModifiedBy = deletedBy;
            await _documentRepository.DeleteAsync(document, cancellationToken);
        }

        // Soft delete the person
        person.ModifiedBy = deletedBy;
        await _personRepository.DeleteAsync(person, cancellationToken);
        return Result.Success();
    }

    private static PersonResponseDto MapToResponseDto(Person person)
    {
        return new PersonResponseDto
        {
            DisplayId = person.DisplayId,
            FirstName = person.FirstName,
            LastName = person.LastName,
            MiddleName = person.MiddleName,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            CivilStatus = person.CivilStatus,
            FullName = person.FullName,
            ProfileImageUrl = person.ProfileImageUrl,
            CreatedOn = person.CreatedOn,
            CreatedBy = person.CreatedBy,
            ModifiedOn = person.ModifiedOn,
            ModifiedBy = person.ModifiedBy,
            Addresses = person.Addresses.Select(a => new AddressResponseDto
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
            Contacts = person.Contacts.Select(c => new ContactResponseDto
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
