using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.School;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;

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
        var displayId = 123456789012L;
        var school = CreateTestSchool(displayId, "Sample Elementary School");

        var schools = new List<School> { school }.BuildMockQueryable();
        _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        var result = await _schoolService.GetByDisplayIdAsync(displayId);

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
        var displayId = 999999999999L;
        var schools = new List<School>().BuildMockQueryable();
        _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        var result = await _schoolService.GetByDisplayIdAsync(displayId);

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
        var query = new PaginationQuery { PageNumber = 1, PageSize = 10 };
        var schools = new List<School>
        {
            CreateTestSchool(100000000001L, "School A"),
            CreateTestSchool(100000000002L, "School B"),
            CreateTestSchool(100000000003L, "School C")
        }.BuildMockQueryable();

        _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        var result = await _schoolService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        // Arrange
        var query = new PaginationQuery { PageNumber = 1, PageSize = 10, SearchTerm = "Elementary" };
        var schools = new List<School>
        {
            CreateTestSchool(100000000001L, "Elementary School A"),
            CreateTestSchool(100000000002L, "High School B"),
            CreateTestSchool(100000000003L, "Elementary School C")
        }.BuildMockQueryable();

        _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        var result = await _schoolService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_WithSortDescending_ReturnsSortedResults()
    {
        // Arrange
        var query = new PaginationQuery { PageNumber = 1, PageSize = 10, SortDescending = true };
        var schools = new List<School>
        {
            CreateTestSchool(100000000001L, "Alpha School"),
            CreateTestSchool(100000000002L, "Zeta School"),
            CreateTestSchool(100000000003L, "Beta School")
        }.BuildMockQueryable();

        _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        var result = await _schoolService.GetPagedAsync(query);

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
        var createDto = new CreateSchoolDto
        {
            SchoolName = "New Test School"
        };

        _schoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<School>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((School s, CancellationToken _) => s);

        // Act
        var result = await _schoolService.CreateAsync(createDto, "TestUser");

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
        var createDto = new CreateSchoolDto
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

        _schoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<School>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((School s, CancellationToken _) => s);

        _addressRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Address a, CancellationToken _) => a);

        // Act
        var result = await _schoolService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.NotNull(result);
        _addressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithContacts_CreatesSchoolWithContacts()
    {
        // Arrange
        var createDto = new CreateSchoolDto
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

        _schoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<School>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((School s, CancellationToken _) => s);

        _contactRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact c, CancellationToken _) => c);

        // Act
        var result = await _schoolService.CreateAsync(createDto, "TestUser");

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
        var displayId = 123456789012L;
        var school = CreateTestSchool(displayId, "Test School");
        school.Addresses = new List<Address>();
        school.Contacts = new List<Contact>();
        school.EmploymentSchools = new List<EmploymentSchool>();

        var schools = new List<School> { school }.BuildMockQueryable();
        _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        _schoolRepositoryMock
            .Setup(r => r.DeleteAsync(school, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _schoolService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _schoolRepositoryMock.Verify(r => r.DeleteAsync(school, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenSchoolDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;

        var schools = new List<School>().BuildMockQueryable();
        _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);

        // Act
        var result = await _schoolService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _schoolRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<School>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static School CreateTestSchool(long displayId, string schoolName)
    {
        var school = new School
        {
            SchoolName = schoolName,
            IsActive = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        // Use reflection to set DisplayId since it has a private setter
        var displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(school, displayId);

        return school;
    }

    #endregion
}
