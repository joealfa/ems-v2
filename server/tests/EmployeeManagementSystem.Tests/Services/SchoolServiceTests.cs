using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.School;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;
using System.Reflection;

namespace EmployeeManagementSystem.Tests.Services;

public class SchoolServiceTests
{
    private readonly Mock<IRepository<School>> _schoolRepositoryMock;
    private readonly Mock<IRepository<Address>> _addressRepositoryMock;
    private readonly Mock<IRepository<Contact>> _contactRepositoryMock;
    private readonly Mock<IRepository<EmploymentSchool>> _employmentSchoolRepositoryMock;
    private readonly SchoolService _schoolService;

    public SchoolServiceTests()
    {
        _schoolRepositoryMock = new Mock<IRepository<School>>();
        _addressRepositoryMock = new Mock<IRepository<Address>>();
        _contactRepositoryMock = new Mock<IRepository<Contact>>();
        _employmentSchoolRepositoryMock = new Mock<IRepository<EmploymentSchool>>();

        _schoolService = new SchoolService(
            _schoolRepositoryMock.Object,
            _addressRepositoryMock.Object,
            _contactRepositoryMock.Object,
            _employmentSchoolRepositoryMock.Object);
    }

    #region GetByDisplayIdAsync Tests

    [Fact]
    public async Task GetByDisplayIdAsync_WhenSchoolExists_ReturnsSchoolResponseDto()
    {
        // Arrange
        long displayId = 123456789012L;
        School school = CreateTestSchool(displayId, "Sample Elementary School");

        IQueryable<School> schools = new List<School> { school }.BuildMockQueryable();
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        Result<SchoolResponseDto> result = await _schoolService.GetByDisplayIdAsync(displayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(displayId, result.Value.DisplayId);
        Assert.Equal(school.SchoolName, result.Value.SchoolName);
    }

    [Fact]
    public async Task GetByDisplayIdAsync_WhenSchoolDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long displayId = 999999999999L;
        IQueryable<School> schools = new List<School>().BuildMockQueryable();
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        Result<SchoolResponseDto> result = await _schoolService.GetByDisplayIdAsync(displayId);

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
        PaginationQuery query = new() { PageNumber = 1, PageSize = 10 };
        IQueryable<School> schools = new List<School>
        {
            CreateTestSchool(100000000001L, "School A"),
            CreateTestSchool(100000000002L, "School B"),
            CreateTestSchool(100000000003L, "School C")
        }.BuildMockQueryable();

        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        PagedResult<SchoolListDto> result = await _schoolService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        // Arrange
        PaginationQuery query = new() { PageNumber = 1, PageSize = 10, SearchTerm = "Elementary" };
        IQueryable<School> schools = new List<School>
        {
            CreateTestSchool(100000000001L, "Elementary School A"),
            CreateTestSchool(100000000002L, "High School B"),
            CreateTestSchool(100000000003L, "Elementary School C")
        }.BuildMockQueryable();

        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        PagedResult<SchoolListDto> result = await _schoolService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_WithSortDescending_ReturnsSortedResults()
    {
        // Arrange
        PaginationQuery query = new() { PageNumber = 1, PageSize = 10, SortDescending = true };
        IQueryable<School> schools = new List<School>
        {
            CreateTestSchool(100000000001L, "Alpha School"),
            CreateTestSchool(100000000002L, "Zeta School"),
            CreateTestSchool(100000000003L, "Beta School")
        }.BuildMockQueryable();

        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        PagedResult<SchoolListDto> result = await _schoolService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("Zeta School", result.Items[0].SchoolName);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedSchool()
    {
        // Arrange
        CreateSchoolDto createDto = new()
        {
            SchoolName = "New Test School"
        };

        _ = _schoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<School>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((School s, CancellationToken _) => s);

        // Act
        Result<SchoolResponseDto> result = await _schoolService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createDto.SchoolName, result.Value.SchoolName);
        Assert.True(result.Value.IsActive);

        _schoolRepositoryMock.Verify(r => r.AddAsync(It.IsAny<School>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithAddresses_CreatesSchoolWithAddresses()
    {
        // Arrange
        CreateSchoolDto createDto = new()
        {
            SchoolName = "New Test School",
            Addresses =
            [
                new CreateAddressDto
                {
                    Address1 = "123 Education Blvd",
                    City = "Quezon City",
                    Province = "Metro Manila",
                    Country = "Philippines"
                }
            ]
        };

        _ = _schoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<School>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((School s, CancellationToken _) => s);

        _ = _addressRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Address a, CancellationToken _) => a);

        // Act
        Result<SchoolResponseDto> result = await _schoolService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.NotNull(result);
        _addressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithContacts_CreatesSchoolWithContacts()
    {
        // Arrange
        CreateSchoolDto createDto = new()
        {
            SchoolName = "New Test School",
            Contacts =
            [
                new CreateContactDto
                {
                    LandLine = "1234567",
                    Email = "school@example.com"
                }
            ]
        };

        _ = _schoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<School>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((School s, CancellationToken _) => s);

        _ = _contactRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact c, CancellationToken _) => c);

        // Act
        Result<SchoolResponseDto> result = await _schoolService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.NotNull(result);
        _contactRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenSchoolExists_ReturnsTrue()
    {
        // Arrange
        long displayId = 123456789012L;
        School school = CreateTestSchool(displayId, "Test School");
        school.Addresses = [];
        school.Contacts = [];
        school.EmploymentSchools = [];

        IQueryable<School> schools = new List<School> { school }.BuildMockQueryable();
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        _ = _schoolRepositoryMock
            .Setup(r => r.DeleteAsync(school, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _schoolService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _schoolRepositoryMock.Verify(r => r.DeleteAsync(school, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenSchoolDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long displayId = 999999999999L;

        IQueryable<School> schools = new List<School>().BuildMockQueryable();
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        Result result = await _schoolService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _schoolRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<School>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static School CreateTestSchool(long displayId, string schoolName)
    {
        School school = new()
        {
            SchoolName = schoolName,
            IsActive = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        // Use reflection to set DisplayId since it has a private setter
        PropertyInfo? displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(school, displayId);

        return school;
    }

    #endregion
}
