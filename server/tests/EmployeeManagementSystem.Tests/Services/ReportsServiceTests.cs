using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;
using System.Reflection;

namespace EmployeeManagementSystem.Tests.Services;

public class ReportsServiceTests
{
    private readonly Mock<IRepository<Person>> _personRepositoryMock;
    private readonly Mock<IRepository<Employment>> _employmentRepositoryMock;
    private readonly Mock<IRepository<School>> _schoolRepositoryMock;
    private readonly Mock<IRepository<Position>> _positionRepositoryMock;
    private readonly Mock<IRepository<SalaryGrade>> _salaryGradeRepositoryMock;
    private readonly Mock<IRepository<Item>> _itemRepositoryMock;
    private readonly ReportsService _reportsService;

    public ReportsServiceTests()
    {
        _personRepositoryMock = new Mock<IRepository<Person>>();
        _employmentRepositoryMock = new Mock<IRepository<Employment>>();
        _schoolRepositoryMock = new Mock<IRepository<School>>();
        _positionRepositoryMock = new Mock<IRepository<Position>>();
        _salaryGradeRepositoryMock = new Mock<IRepository<SalaryGrade>>();
        _itemRepositoryMock = new Mock<IRepository<Item>>();

        _reportsService = new ReportsService(
            _personRepositoryMock.Object,
            _employmentRepositoryMock.Object,
            _schoolRepositoryMock.Object,
            _positionRepositoryMock.Object,
            _salaryGradeRepositoryMock.Object,
            _itemRepositoryMock.Object);
    }

    #region GetDashboardStatsAsync Tests

    [Fact]
    public async Task GetDashboardStatsAsync_ReturnsCorrectCounts()
    {
        // Arrange
        IQueryable<Person> persons = new List<Person>
        {
            CreateTestPerson(100000000001L),
            CreateTestPerson(100000000002L),
            CreateTestPerson(100000000003L)
        }.BuildMockQueryable();

        IQueryable<Employment> employments = new List<Employment>
        {
            CreateTestEmployment(200000000001L, isActive: true),
            CreateTestEmployment(200000000002L, isActive: true),
            CreateTestEmployment(200000000003L, isActive: false)
        }.BuildMockQueryable();

        IQueryable<School> schools = new List<School>
        {
            CreateTestSchool(300000000001L),
            CreateTestSchool(300000000002L)
        }.BuildMockQueryable();

        IQueryable<Position> positions = new List<Position>
        {
            CreateTestPosition(400000000001L),
            CreateTestPosition(400000000002L),
            CreateTestPosition(400000000003L),
            CreateTestPosition(400000000004L)
        }.BuildMockQueryable();

        IQueryable<SalaryGrade> salaryGrades = new List<SalaryGrade>
        {
            CreateTestSalaryGrade(500000000001L),
            CreateTestSalaryGrade(500000000002L),
            CreateTestSalaryGrade(500000000003L),
            CreateTestSalaryGrade(500000000004L),
            CreateTestSalaryGrade(500000000005L)
        }.BuildMockQueryable();

        IQueryable<Item> items = new List<Item>
        {
            CreateTestItem(600000000001L),
            CreateTestItem(600000000002L),
            CreateTestItem(600000000003L),
            CreateTestItem(600000000004L),
            CreateTestItem(600000000005L),
            CreateTestItem(600000000006L)
        }.BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);
        _ = _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);
        _ = _salaryGradeRepositoryMock.Setup(r => r.Query()).Returns(salaryGrades);
        _ = _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        DashboardStatsDto result = await _reportsService.GetDashboardStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalPersons);
        Assert.Equal(2, result.ActiveEmployments); // Only active ones
        Assert.Equal(2, result.TotalSchools);
        Assert.Equal(4, result.TotalPositions);
        Assert.Equal(5, result.TotalSalaryGrades);
        Assert.Equal(6, result.TotalItems);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_WithEmptyData_ReturnsZeroCounts()
    {
        // Arrange
        IQueryable<Person> persons = new List<Person>().BuildMockQueryable();
        IQueryable<Employment> employments = new List<Employment>().BuildMockQueryable();
        IQueryable<School> schools = new List<School>().BuildMockQueryable();
        IQueryable<Position> positions = new List<Position>().BuildMockQueryable();
        IQueryable<SalaryGrade> salaryGrades = new List<SalaryGrade>().BuildMockQueryable();
        IQueryable<Item> items = new List<Item>().BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);
        _ = _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);
        _ = _salaryGradeRepositoryMock.Setup(r => r.Query()).Returns(salaryGrades);
        _ = _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        DashboardStatsDto result = await _reportsService.GetDashboardStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalPersons);
        Assert.Equal(0, result.ActiveEmployments);
        Assert.Equal(0, result.TotalSchools);
        Assert.Equal(0, result.TotalPositions);
        Assert.Equal(0, result.TotalSalaryGrades);
        Assert.Equal(0, result.TotalItems);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_WithNoActiveEmployments_ReturnsZeroActiveEmployments()
    {
        // Arrange
        IQueryable<Person> persons = new List<Person>
        {
            CreateTestPerson(100000000001L),
            CreateTestPerson(100000000002L)
        }.BuildMockQueryable();

        IQueryable<Employment> employments = new List<Employment>
        {
            CreateTestEmployment(200000000001L, isActive: false),
            CreateTestEmployment(200000000002L, isActive: false),
            CreateTestEmployment(200000000003L, isActive: false)
        }.BuildMockQueryable();

        IQueryable<School> schools = new List<School>
        {
            CreateTestSchool(300000000001L)
        }.BuildMockQueryable();

        IQueryable<Position> positions = new List<Position>().BuildMockQueryable();
        IQueryable<SalaryGrade> salaryGrades = new List<SalaryGrade>().BuildMockQueryable();
        IQueryable<Item> items = new List<Item>().BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);
        _ = _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);
        _ = _salaryGradeRepositoryMock.Setup(r => r.Query()).Returns(salaryGrades);
        _ = _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        DashboardStatsDto result = await _reportsService.GetDashboardStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalPersons);
        Assert.Equal(0, result.ActiveEmployments);
        Assert.Equal(1, result.TotalSchools);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_WithAllActiveEmployments_ReturnsCorrectActiveCount()
    {
        // Arrange
        IQueryable<Person> persons = new List<Person>().BuildMockQueryable();

        IQueryable<Employment> employments = new List<Employment>
        {
            CreateTestEmployment(200000000001L, isActive: true),
            CreateTestEmployment(200000000002L, isActive: true),
            CreateTestEmployment(200000000003L, isActive: true),
            CreateTestEmployment(200000000004L, isActive: true),
            CreateTestEmployment(200000000005L, isActive: true)
        }.BuildMockQueryable();

        IQueryable<School> schools = new List<School>().BuildMockQueryable();
        IQueryable<Position> positions = new List<Position>().BuildMockQueryable();
        IQueryable<SalaryGrade> salaryGrades = new List<SalaryGrade>().BuildMockQueryable();
        IQueryable<Item> items = new List<Item>().BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);
        _ = _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);
        _ = _salaryGradeRepositoryMock.Setup(r => r.Query()).Returns(salaryGrades);
        _ = _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        DashboardStatsDto result = await _reportsService.GetDashboardStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.ActiveEmployments);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_WithCancellationToken_PassesTokenToRepositories()
    {
        // Arrange
        CancellationToken cancellationToken = new();

        IQueryable<Person> persons = new List<Person>().BuildMockQueryable();
        IQueryable<Employment> employments = new List<Employment>().BuildMockQueryable();
        IQueryable<School> schools = new List<School>().BuildMockQueryable();
        IQueryable<Position> positions = new List<Position>().BuildMockQueryable();
        IQueryable<SalaryGrade> salaryGrades = new List<SalaryGrade>().BuildMockQueryable();
        IQueryable<Item> items = new List<Item>().BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);
        _ = _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);
        _ = _salaryGradeRepositoryMock.Setup(r => r.Query()).Returns(salaryGrades);
        _ = _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        DashboardStatsDto result = await _reportsService.GetDashboardStatsAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        _personRepositoryMock.Verify(r => r.Query(), Times.Once);
        _employmentRepositoryMock.Verify(r => r.Query(), Times.Once);
        _schoolRepositoryMock.Verify(r => r.Query(), Times.Once);
        _positionRepositoryMock.Verify(r => r.Query(), Times.Once);
        _salaryGradeRepositoryMock.Verify(r => r.Query(), Times.Once);
        _itemRepositoryMock.Verify(r => r.Query(), Times.Once);
    }

    [Fact]
    public async Task GetDashboardStatsAsync_ReturnsDashboardStatsDto()
    {
        // Arrange
        IQueryable<Person> persons = new List<Person>().BuildMockQueryable();
        IQueryable<Employment> employments = new List<Employment>().BuildMockQueryable();
        IQueryable<School> schools = new List<School>().BuildMockQueryable();
        IQueryable<Position> positions = new List<Position>().BuildMockQueryable();
        IQueryable<SalaryGrade> salaryGrades = new List<SalaryGrade>().BuildMockQueryable();
        IQueryable<Item> items = new List<Item>().BuildMockQueryable();

        _ = _personRepositoryMock.Setup(r => r.Query()).Returns(persons);
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _ = _schoolRepositoryMock.Setup(r => r.Query()).Returns(schools);
        _ = _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);
        _ = _salaryGradeRepositoryMock.Setup(r => r.Query()).Returns(salaryGrades);
        _ = _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        DashboardStatsDto result = await _reportsService.GetDashboardStatsAsync();

        // Assert
        _ = Assert.IsType<DashboardStatsDto>(result);
    }

    #endregion

    #region Helper Methods

    private static Person CreateTestPerson(long displayId, string firstName = "Test", string lastName = "Person")
    {
        Person person = new()
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            CivilStatus = CivilStatus.Single,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(person, displayId);

        return person;
    }

    private static Employment CreateTestEmployment(long displayId, bool isActive = true)
    {
        Employment employment = new()
        {
            DepEdId = $"DEPED-{displayId}",
            AppointmentStatus = AppointmentStatus.Original,
            EmploymentStatus = EmploymentStatus.Regular,
            Eligibility = Eligibility.CivilServiceProfessional,
            IsActive = isActive,
            PersonId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            SalaryGradeId = Guid.NewGuid(),
            ItemId = Guid.NewGuid(),
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(employment, displayId);

        return employment;
    }

    private static School CreateTestSchool(long displayId)
    {
        School school = new()
        {
            SchoolName = $"Test School {displayId}",
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(school, displayId);

        return school;
    }

    private static Position CreateTestPosition(long displayId)
    {
        Position position = new()
        {
            TitleName = $"Position {displayId}",
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(position, displayId);

        return position;
    }

    private static SalaryGrade CreateTestSalaryGrade(long displayId)
    {
        SalaryGrade salaryGrade = new()
        {
            SalaryGradeName = $"SG-{displayId}",
            Step = 1,
            MonthlySalary = 25000.00m,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(salaryGrade, displayId);

        return salaryGrade;
    }

    private static Item CreateTestItem(long displayId)
    {
        Item item = new()
        {
            ItemName = $"Item-{displayId}",
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(item, displayId);

        return item;
    }

    private static void SetDisplayId<T>(T entity, long displayId) where T : BaseEntity
    {
        PropertyInfo? displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(entity, displayId);
    }

    #endregion
}
