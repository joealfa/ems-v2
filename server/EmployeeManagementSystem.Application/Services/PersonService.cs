using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Person;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Mappings;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    IRepository<Document> documentRepository,
    ILogger<PersonService> logger) : IPersonService
{
    private readonly IRepository<Person> _personRepository = personRepository;
    private readonly IRepository<Address> _addressRepository = addressRepository;
    private readonly IRepository<Contact> _contactRepository = contactRepository;
    private readonly IRepository<Document> _documentRepository = documentRepository;
    private readonly ILogger<PersonService> _logger = logger;

    /// <inheritdoc />
    public async Task<Result<PersonResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        Person? person = await _personRepository.Query()
            .Include(p => p.Addresses.Where(a => !a.IsDeleted))
            .Include(p => p.Contacts.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(p => p.DisplayId == displayId, cancellationToken);

        return person == null
            ? Result<PersonResponseDto>.NotFound("Person not found.")
            : Result<PersonResponseDto>.Success(person.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<PagedResult<PersonListDto>> GetPagedAsync(PersonPaginationQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Person> queryable = _personRepository.Query();

        // Apply search term filter (searches across name fields)
        // Split by spaces to handle multi-word searches like "John Doe"
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            string[] searchTerms = query.SearchTerm.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string term in searchTerms)
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
            string[] filterTerms = query.FullNameFilter.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string term in filterTerms)
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

        int totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(p => p.LastName).ThenByDescending(p => p.FirstName)
            : queryable.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

        List<PersonListDto> items = await queryable
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
                HasProfileImage = p.HasProfileImage,
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
        _logger.LogInformation("Creating new person: {FirstName} {LastName} by user {CreatedBy}",
            dto.FirstName, dto.LastName, createdBy);

        Person person = new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            CivilStatus = dto.CivilStatus,
            CreatedBy = createdBy
        };

        _ = await _personRepository.AddAsync(person, cancellationToken);

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
                    PersonId = person.Id,
                    CreatedBy = createdBy
                };
                _ = await _addressRepository.AddAsync(address, cancellationToken);
                person.Addresses.Add(address);
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
                    PersonId = person.Id,
                    CreatedBy = createdBy
                };
                _ = await _contactRepository.AddAsync(contact, cancellationToken);
                person.Contacts.Add(contact);
            }
        }

        _logger.LogInformation("Person created successfully: DisplayId {DisplayId}, Name: {FullName} by user {CreatedBy}",
            person.DisplayId, person.FullName, createdBy);

        return Result<PersonResponseDto>.Success(person.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result<PersonResponseDto>> UpdateAsync(long displayId, UpdatePersonDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating person with DisplayId {DisplayId} by user {ModifiedBy}", displayId, modifiedBy);

        Person? person = await _personRepository.Query()
            .Include(p => p.Addresses.Where(a => !a.IsDeleted))
            .Include(p => p.Contacts.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(p => p.DisplayId == displayId, cancellationToken);

        if (person == null)
        {
            _logger.LogWarning("Person update failed - DisplayId {DisplayId} not found", displayId);
            return Result<PersonResponseDto>.NotFound("Person not found.");
        }

        person.FirstName = dto.FirstName;
        person.LastName = dto.LastName;
        person.MiddleName = dto.MiddleName;
        person.DateOfBirth = dto.DateOfBirth;
        person.Gender = dto.Gender;
        person.CivilStatus = dto.CivilStatus;
        person.ModifiedBy = modifiedBy;

        // Sync addresses if provided
        if (dto.Addresses != null)
        {
            HashSet<long> addressDisplayIds = dto.Addresses
                .Where(a => a.DisplayId.HasValue && a.DisplayId.Value > 0)
                .Select(a => a.DisplayId!.Value)
                .ToHashSet();

            // Soft-delete addresses not in the list
            foreach (Address existing in person.Addresses.Where(a => !addressDisplayIds.Contains(a.DisplayId)).ToList())
            {
                existing.ModifiedBy = modifiedBy;
                await _addressRepository.DeleteAsync(existing, cancellationToken);
                _ = person.Addresses.Remove(existing);
            }

            // Update existing / create new
            foreach (UpsertAddressDto addrDto in dto.Addresses)
            {
                if (addrDto.DisplayId.HasValue && addrDto.DisplayId.Value > 0)
                {
                    // Update existing
                    Address? existing = person.Addresses.FirstOrDefault(a => a.DisplayId == addrDto.DisplayId);
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
                    // Create new - EF relationship fixup will add to person.Addresses automatically
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
                        PersonId = person.Id,
                        CreatedBy = modifiedBy
                    };
                    _ = await _addressRepository.AddAsync(newAddress, cancellationToken);
                }
            }
        }

        // Sync contacts if provided
        if (dto.Contacts != null)
        {
            HashSet<long> contactDisplayIds = dto.Contacts
                .Where(c => c.DisplayId.HasValue && c.DisplayId.Value > 0)
                .Select(c => c.DisplayId!.Value)
                .ToHashSet();

            // Soft-delete contacts not in the list
            foreach (Contact existing in person.Contacts.Where(c => !contactDisplayIds.Contains(c.DisplayId)).ToList())
            {
                existing.ModifiedBy = modifiedBy;
                await _contactRepository.DeleteAsync(existing, cancellationToken);
                _ = person.Contacts.Remove(existing);
            }

            // Update existing / create new
            foreach (UpsertContactDto contactDto in dto.Contacts)
            {
                if (contactDto.DisplayId.HasValue && contactDto.DisplayId.Value > 0)
                {
                    // Update existing
                    Contact? existing = person.Contacts.FirstOrDefault(c => c.DisplayId == contactDto.DisplayId);
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
                    // Create new - EF relationship fixup will add to person.Contacts automatically
                    Contact newContact = new()
                    {
                        Mobile = contactDto.Mobile,
                        LandLine = contactDto.LandLine,
                        Fax = contactDto.Fax,
                        Email = contactDto.Email,
                        ContactType = contactDto.ContactType,
                        PersonId = person.Id,
                        CreatedBy = modifiedBy
                    };
                    _ = await _contactRepository.AddAsync(newContact, cancellationToken);
                }
            }
        }

        await _personRepository.UpdateAsync(person, cancellationToken);

        _logger.LogInformation("Person updated successfully: DisplayId {DisplayId}, Name: {FullName} by user {ModifiedBy}",
            person.DisplayId, person.FullName, modifiedBy);

        return Result<PersonResponseDto>.Success(person.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting person with DisplayId {DisplayId} by user {DeletedBy}", displayId, deletedBy);

        Person? person = await _personRepository.Query()
            .Include(p => p.Addresses.Where(a => !a.IsDeleted))
            .Include(p => p.Contacts.Where(c => !c.IsDeleted))
            .Include(p => p.Documents.Where(d => !d.IsDeleted))
            .FirstOrDefaultAsync(p => p.DisplayId == displayId, cancellationToken);

        if (person == null)
        {
            _logger.LogWarning("Person delete failed - DisplayId {DisplayId} not found", displayId);
            return Result.NotFound("Person not found.");
        }

        // Cascade soft delete to related addresses
        foreach (Address address in person.Addresses)
        {
            address.ModifiedBy = deletedBy;
            await _addressRepository.DeleteAsync(address, cancellationToken);
        }

        // Cascade soft delete to related contacts
        foreach (Contact contact in person.Contacts)
        {
            contact.ModifiedBy = deletedBy;
            await _contactRepository.DeleteAsync(contact, cancellationToken);
        }

        // Cascade soft delete to related documents
        foreach (Document document in person.Documents)
        {
            document.ModifiedBy = deletedBy;
            await _documentRepository.DeleteAsync(document, cancellationToken);
        }

        // Soft delete the person
        person.ModifiedBy = deletedBy;
        await _personRepository.DeleteAsync(person, cancellationToken);

        _logger.LogInformation("Person deleted successfully: DisplayId {DisplayId}, Name: {FullName} by user {DeletedBy}. " +
            "Cascade deleted {AddressCount} addresses, {ContactCount} contacts, {DocumentCount} documents",
            person.DisplayId, person.FullName, deletedBy, person.Addresses.Count, person.Contacts.Count, person.Documents.Count);

        return Result.Success();
    }
}
