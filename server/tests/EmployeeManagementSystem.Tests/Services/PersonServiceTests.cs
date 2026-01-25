using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Person;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;
using System.Reflection;

namespace EmployeeManagementSystem.Tests.Services;

public class PersonServiceTests
{
    private readonly Mock<IRepository<Person>> _personRepositoryMock;
    private readonly Mock<IRepository<Address>> _addressRepositoryMock;
    private readonly Mock<IRepository<Contact>> _contactRepositoryMock;
    private readonly Mock<IRepository<Document>> _documentRepositoryMock;
    private readonly PersonService _personService;

    public PersonServiceTests()
    {
        _personRepositoryMock = new Mock<IRepository<Person>>();
        _addressRepositoryMock = new Mock<IRepository<Address>>();
        _contactRepositoryMock = new Mock<IRepository<Contact>>();
        _documentRepositoryMock = new Mock<IRepository<Document>>();

        _personService = new PersonService(
            _personRepositoryMock.Object,
            _addressRepositoryMock.Object,
            _contactRepositoryMock.Object,
            _documentRepositoryMock.Object);
    }

    #region GetByDisplayIdAsync Tests

    [Fact]
    public async Task GetByDisplayIdAsync_WhenPersonExists_ReturnsPersonResponseDto()
    {
        // Arrange
        long displayId = 123456789012L;
        Person person = CreateTestPerson(displayId);

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        Result<PersonResponseDto> result = await _personService.GetByDisplayIdAsync(displayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(displayId, result.Value.DisplayId);
        Assert.Equal(person.FirstName, result.Value.FirstName);
        Assert.Equal(person.LastName, result.Value.LastName);
    }

    [Fact]
    public async Task GetByDisplayIdAsync_WhenPersonDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long displayId = 999999999999L;
        IQueryable<Person> persons = new List<Person>().BuildMockQueryable();
        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        Result<PersonResponseDto> result = await _personService.GetByDisplayIdAsync(displayId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    #endregion

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResult()
    {
        // Arrange
        PersonPaginationQuery query = new() { PageNumber = 1, PageSize = 10 };
        IQueryable<Person> persons = new List<Person>
        {
            CreateTestPerson(100000000001L, "John", "Doe"),
            CreateTestPerson(100000000002L, "Jane", "Smith"),
            CreateTestPerson(100000000003L, "Bob", "Johnson")
        }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        PagedResult<PersonListDto> result = await _personService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        // Arrange
        PersonPaginationQuery query = new() { PageNumber = 1, PageSize = 10, SearchTerm = "John" };
        IQueryable<Person> persons = new List<Person>
        {
            CreateTestPerson(100000000001L, "John", "Doe"),
            CreateTestPerson(100000000002L, "Jane", "Smith"),
            CreateTestPerson(100000000003L, "Bob", "Johnson")
        }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        PagedResult<PersonListDto> result = await _personService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount); // John Doe and Bob Johnson
    }

    [Fact]
    public async Task GetPagedAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        PersonPaginationQuery query = new() { PageNumber = 2, PageSize = 2 };
        IQueryable<Person> persons = new List<Person>
        {
            CreateTestPerson(100000000001L, "Alice", "Anderson"),
            CreateTestPerson(100000000002L, "Bob", "Brown"),
            CreateTestPerson(100000000003L, "Charlie", "Clark"),
            CreateTestPerson(100000000004L, "David", "Davis")
        }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        PagedResult<PersonListDto> result = await _personService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.PageNumber);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedPerson()
    {
        // Arrange
        CreatePersonDto createDto = new()
        {
            FirstName = "Test",
            LastName = "Person",
            MiddleName = "Middle",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            CivilStatus = CivilStatus.Single
        };

        _ = _personRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person p, CancellationToken _) => p);

        // Act
        Result<PersonResponseDto> result = await _personService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createDto.FirstName, result.Value.FirstName);
        Assert.Equal(createDto.LastName, result.Value.LastName);
        Assert.Equal(createDto.MiddleName, result.Value.MiddleName);
        Assert.Equal(createDto.DateOfBirth, result.Value.DateOfBirth);
        Assert.Equal(createDto.Gender, result.Value.Gender);
        Assert.Equal(createDto.CivilStatus, result.Value.CivilStatus);

        _personRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithAddresses_CreatesPersonWithAddresses()
    {
        // Arrange
        CreatePersonDto createDto = new()
        {
            FirstName = "Test",
            LastName = "Person",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            CivilStatus = CivilStatus.Single,
            Addresses =
            [
                new CreateAddressDto
                {
                    Address1 = "123 Main St",
                    City = "Manila",
                    Province = "Metro Manila",
                    Country = "Philippines"
                }
            ]
        };

        _ = _personRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person p, CancellationToken _) => p);

        _ = _addressRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Address a, CancellationToken _) => a);

        // Act
        Result<PersonResponseDto> result = await _personService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _addressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithContacts_CreatesPersonWithContacts()
    {
        // Arrange
        CreatePersonDto createDto = new()
        {
            FirstName = "Test",
            LastName = "Person",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            CivilStatus = CivilStatus.Single,
            Contacts =
            [
                new CreateContactDto
                {
                    Mobile = "09123456789",
                    Email = "test@example.com"
                }
            ]
        };

        _ = _personRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person p, CancellationToken _) => p);

        _ = _contactRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact c, CancellationToken _) => c);

        // Act
        Result<PersonResponseDto> result = await _personService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _contactRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenPersonExists_ReturnsTrue()
    {
        // Arrange
        long displayId = 123456789012L;
        Person person = CreateTestPerson(displayId);
        person.Addresses = [];
        person.Contacts = [];
        person.Documents = [];

        IQueryable<Person> persons = new List<Person> { person }.BuildMockQueryable();
        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        _ = _personRepositoryMock
            .Setup(r => r.DeleteAsync(person, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _personService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _personRepositoryMock.Verify(r => r.DeleteAsync(person, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenPersonDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long displayId = 999999999999L;

        IQueryable<Person> persons = new List<Person>().BuildMockQueryable();
        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);

        // Act
        Result result = await _personService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _personRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static Person CreateTestPerson(
        long displayId,
        string firstName = "Test",
        string lastName = "Person",
        string? middleName = null)
    {
        Person person = new()
        {
            FirstName = firstName,
            LastName = lastName,
            MiddleName = middleName,
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            CivilStatus = CivilStatus.Single,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        // Use reflection to set DisplayId since it has a private setter
        PropertyInfo? displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(person, displayId);

        return person;
    }

    #endregion
}
