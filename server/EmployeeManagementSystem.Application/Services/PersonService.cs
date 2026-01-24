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
public class PersonService : IPersonService
{
    private readonly IRepository<Person> _personRepository;
    private readonly IRepository<Address> _addressRepository;
    private readonly IRepository<Contact> _contactRepository;
    private readonly IRepository<Document> _documentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonService"/> class.
    /// </summary>
    public PersonService(
        IRepository<Person> personRepository,
        IRepository<Address> addressRepository,
        IRepository<Contact> contactRepository,
        IRepository<Document> documentRepository)
    {
        _personRepository = personRepository;
        _addressRepository = addressRepository;
        _contactRepository = contactRepository;
        _documentRepository = documentRepository;
    }

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
    public async Task<PagedResult<PersonListDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _personRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.ToLower();
            queryable = queryable.Where(p =>
                p.FirstName.ToLower().Contains(searchTerm) ||
                p.LastName.ToLower().Contains(searchTerm) ||
                (p.MiddleName != null && p.MiddleName.ToLower().Contains(searchTerm)));
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
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
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
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.UtcNow
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
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.UtcNow
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
        person.ModifiedOn = DateTime.UtcNow;

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
            address.ModifiedOn = DateTime.UtcNow;
            await _addressRepository.DeleteAsync(address, cancellationToken);
        }

        // Cascade soft delete to related contacts
        foreach (var contact in person.Contacts)
        {
            contact.ModifiedBy = deletedBy;
            contact.ModifiedOn = DateTime.UtcNow;
            await _contactRepository.DeleteAsync(contact, cancellationToken);
        }

        // Cascade soft delete to related documents
        foreach (var document in person.Documents)
        {
            document.ModifiedBy = deletedBy;
            document.ModifiedOn = DateTime.UtcNow;
            await _documentRepository.DeleteAsync(document, cancellationToken);
        }

        // Soft delete the person
        person.ModifiedBy = deletedBy;
        person.ModifiedOn = DateTime.UtcNow;
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
