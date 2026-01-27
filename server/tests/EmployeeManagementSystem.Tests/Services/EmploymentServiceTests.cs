using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;
using System.Reflection;

namespace EmployeeManagementSystem.Tests.Services;

public class EmploymentServiceTests
{
    private readonly Mock<IRepository<Employment>> _employmentRepositoryMock;
    private readonly Mock<IRepository<EmploymentSchool>> _employmentSchoolRepositoryMock;
    private readonly Mock<IRepository<Person>> _personRepositoryMock;
    private readonly Mock<IRepository<Position>> _positionRepositoryMock;
    private readonly Mock<IRepository<SalaryGrade>> _salaryGradeRepositoryMock;
    private readonly Mock<IRepository<Item>> _itemRepositoryMock;
    private readonly Mock<IRepository<School>> _schoolRepositoryMock;
    private readonly EmploymentService _employmentService;

    public EmploymentServiceTests()
    {
        _employmentRepositoryMock = new Mock<IRepository<Employment>>();
        _employmentSchoolRepositoryMock = new Mock<IRepository<EmploymentSchool>>();
        _personRepositoryMock = new Mock<IRepository<Person>>();
        _positionRepositoryMock = new Mock<IRepository<Position>>();
        _salaryGradeRepositoryMock = new Mock<IRepository<SalaryGrade>>();
        _itemRepositoryMock = new Mock<IRepository<Item>>();
        _schoolRepositoryMock = new Mock<IRepository<School>>();

        _employmentService = new EmploymentService(
            _employmentRepositoryMock.Object,
            _employmentSchoolRepositoryMock.Object,
            _personRepositoryMock.Object,
            _positionRepositoryMock.Object,
            _salaryGradeRepositoryMock.Object,
            _itemRepositoryMock.Object,
            _schoolRepositoryMock.Object);
    }

    #region GetByDisplayIdAsync Tests

    [Fact]
    public async Task GetByDisplayIdAsync_WhenEmploymentExists_ReturnsEmploymentResponseDto()
    {
        // Arrange
        long displayId = 100000000001L;
        Employment employment = CreateTestEmployment(displayId);

        IQueryable<Employment> employments = new List<Employment> { employment }.BuildMockQueryable();
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        Result<EmploymentResponseDto> result = await _employmentService.GetByDisplayIdAsync(displayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(displayId, result.Value.DisplayId);
    }

    [Fact]
    public async Task GetByDisplayIdAsync_WhenEmploymentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long displayId = 999999999999L;
        IQueryable<Employment> employments = new List<Employment>().BuildMockQueryable();
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        Result<EmploymentResponseDto> result = await _employmentService.GetByDisplayIdAsync(displayId);

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
        EmploymentPaginationQuery query = new() { PageNumber = 1, PageSize = 10 };
        IQueryable<Employment> employments = new List<Employment>
        {
            CreateTestEmployment(100000000001L),
            CreateTestEmployment(100000000002L),
            CreateTestEmployment(100000000003L)
        }.BuildMockQueryable();

        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        PagedResult<EmploymentListDto> result = await _employmentService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        // Arrange
        EmploymentPaginationQuery query = new() { PageNumber = 1, PageSize = 10, SearchTerm = "John" };
        IQueryable<Employment> employments = new List<Employment>
        {
            CreateTestEmployment(100000000001L, personFirstName: "John", personLastName: "Doe"),
            CreateTestEmployment(100000000002L, personFirstName: "Jane", personLastName: "Smith"),
            CreateTestEmployment(100000000003L, personFirstName: "Bob", personLastName: "Johnson")
        }.BuildMockQueryable();

        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        PagedResult<EmploymentListDto> result = await _employmentService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount); // John Doe and Bob Johnson
    }

    [Fact]
    public async Task GetPagedAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        EmploymentPaginationQuery query = new() { PageNumber = 2, PageSize = 2 };
        IQueryable<Employment> employments = new List<Employment>
        {
            CreateTestEmployment(100000000001L, personFirstName: "Alice", personLastName: "Anderson"),
            CreateTestEmployment(100000000002L, personFirstName: "Bob", personLastName: "Brown"),
            CreateTestEmployment(100000000003L, personFirstName: "Charlie", personLastName: "Clark"),
            CreateTestEmployment(100000000004L, personFirstName: "David", personLastName: "Davis")
        }.BuildMockQueryable();

        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        PagedResult<EmploymentListDto> result = await _employmentService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.PageNumber);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedEmployment()
    {
        // Arrange
        Person person = CreateTestPerson(100000000001L);
        Position position = CreateTestPosition(200000000001L);
        SalaryGrade salaryGrade = CreateTestSalaryGrade(300000000001L);
        Item item = CreateTestItem(400000000001L);

        _ = _personRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(person.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);
        _ = _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(position.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);
        _ = _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(salaryGrade.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(salaryGrade);
        _ = _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(item.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _ = _employmentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employment e, CancellationToken _) => e);

        CreateEmploymentDto createDto = new()
        {
            PersonDisplayId = person.DisplayId,
            PositionDisplayId = position.DisplayId,
            SalaryGradeDisplayId = salaryGrade.DisplayId,
            ItemDisplayId = item.DisplayId,
            DepEdId = "DEPED-001",
            AppointmentStatus = AppointmentStatus.Original,
            EmploymentStatus = EmploymentStatus.Regular,
            Eligibility = Eligibility.CivilServiceProfessional
        };

        // Act
        Result<EmploymentResponseDto> result = await _employmentService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createDto.DepEdId, result.Value.DepEdId);
        Assert.Equal(createDto.AppointmentStatus, result.Value.AppointmentStatus);
        Assert.Equal(createDto.EmploymentStatus, result.Value.EmploymentStatus);

        _employmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenPersonNotFound_ReturnsBadRequest()
    {
        // Arrange
        _ = _personRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person?)null);

        CreateEmploymentDto createDto = new()
        {
            PersonDisplayId = 999999999999L,
            PositionDisplayId = 200000000001L,
            SalaryGradeDisplayId = 300000000001L,
            ItemDisplayId = 400000000001L,
            AppointmentStatus = AppointmentStatus.Original,
            EmploymentStatus = EmploymentStatus.Regular,
            Eligibility = Eligibility.CivilServiceProfessional
        };

        // Act
        Result<EmploymentResponseDto> result = await _employmentService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.BadRequest, result.FailureType);
    }

    [Fact]
    public async Task CreateAsync_WhenPositionNotFound_ReturnsBadRequest()
    {
        // Arrange
        Person person = CreateTestPerson(100000000001L);

        _ = _personRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(person.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);
        _ = _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position?)null);

        CreateEmploymentDto createDto = new()
        {
            PersonDisplayId = person.DisplayId,
            PositionDisplayId = 999999999999L,
            SalaryGradeDisplayId = 300000000001L,
            ItemDisplayId = 400000000001L,
            AppointmentStatus = AppointmentStatus.Original,
            EmploymentStatus = EmploymentStatus.Regular,
            Eligibility = Eligibility.CivilServiceProfessional
        };

        // Act
        Result<EmploymentResponseDto> result = await _employmentService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.BadRequest, result.FailureType);
    }

    [Fact]
    public async Task CreateAsync_WithSchools_CreatesEmploymentWithSchoolAssignments()
    {
        // Arrange
        Person person = CreateTestPerson(100000000001L);
        Position position = CreateTestPosition(200000000001L);
        SalaryGrade salaryGrade = CreateTestSalaryGrade(300000000001L);
        Item item = CreateTestItem(400000000001L);
        School school = CreateTestSchool(500000000001L);

        _ = _personRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(person.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);
        _ = _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(position.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);
        _ = _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(salaryGrade.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(salaryGrade);
        _ = _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(item.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _ = _schoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(school.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(school);

        _ = _employmentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employment e, CancellationToken _) => e);
        _ = _employmentSchoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmploymentSchool es, CancellationToken _) =>
            {
                // The service sets the School navigation property after adding to the collection
                es.School = school;
                return es;
            });

        CreateEmploymentDto createDto = new()
        {
            PersonDisplayId = person.DisplayId,
            PositionDisplayId = position.DisplayId,
            SalaryGradeDisplayId = salaryGrade.DisplayId,
            ItemDisplayId = item.DisplayId,
            AppointmentStatus = AppointmentStatus.Original,
            EmploymentStatus = EmploymentStatus.Regular,
            Eligibility = Eligibility.CivilServiceProfessional,
            Schools =
            [
                new CreateEmploymentSchoolDto
                {
                    SchoolDisplayId = school.DisplayId,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    IsCurrent = true
                }
            ]
        };

        // Act
        Result<EmploymentResponseDto> result = await _employmentService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _ = Assert.Single(result.Value.Schools);
        _employmentSchoolRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenEmploymentExists_DeletesSuccessfully()
    {
        // Arrange
        long displayId = 100000000001L;
        Employment employment = CreateTestEmployment(displayId);
        employment.EmploymentSchools = [];

        IQueryable<Employment> employments = new List<Employment> { employment }.BuildMockQueryable();
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _ = _employmentRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _employmentService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _employmentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenEmploymentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long displayId = 999999999999L;
        IQueryable<Employment> employments = new List<Employment>().BuildMockQueryable();
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        Result result = await _employmentService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    [Fact]
    public async Task DeleteAsync_WithSchoolAssignments_CascadeDeletesSchools()
    {
        // Arrange
        long displayId = 100000000001L;
        Employment employment = CreateTestEmployment(displayId);
        EmploymentSchool employmentSchool = new()
        {
            EmploymentId = employment.Id,
            SchoolId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };
        employment.EmploymentSchools = [employmentSchool];

        IQueryable<Employment> employments = new List<Employment> { employment }.BuildMockQueryable();
        _ = _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _ = _employmentRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _ = _employmentSchoolRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _employmentService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _employmentSchoolRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()), Times.Once);
        _employmentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region AddSchoolAssignmentAsync Tests

    [Fact]
    public async Task AddSchoolAssignmentAsync_WithValidData_ReturnsCreatedSchoolAssignment()
    {
        // Arrange
        long employmentDisplayId = 100000000001L;
        long schoolDisplayId = 500000000001L;
        Employment employment = CreateTestEmployment(employmentDisplayId);
        School school = CreateTestSchool(schoolDisplayId);

        _ = _employmentRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(employmentDisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employment);
        _ = _schoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(schoolDisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(school);
        _ = _employmentSchoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmploymentSchool es, CancellationToken _) => es);

        CreateEmploymentSchoolDto createDto = new()
        {
            SchoolDisplayId = schoolDisplayId,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true
        };

        // Act
        Result<EmploymentSchoolResponseDto> result = await _employmentService.AddSchoolAssignmentAsync(employmentDisplayId, createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(schoolDisplayId, result.Value.SchoolDisplayId);
        Assert.Equal(school.SchoolName, result.Value.SchoolName);
    }

    [Fact]
    public async Task AddSchoolAssignmentAsync_WhenEmploymentNotFound_ReturnsNotFound()
    {
        // Arrange
        _ = _employmentRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employment?)null);

        CreateEmploymentSchoolDto createDto = new()
        {
            SchoolDisplayId = 500000000001L,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true
        };

        // Act
        Result<EmploymentSchoolResponseDto> result = await _employmentService.AddSchoolAssignmentAsync(999999999999L, createDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    [Fact]
    public async Task AddSchoolAssignmentAsync_WhenSchoolNotFound_ReturnsNotFound()
    {
        // Arrange
        long employmentDisplayId = 100000000001L;
        Employment employment = CreateTestEmployment(employmentDisplayId);

        _ = _employmentRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(employmentDisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employment);
        _ = _schoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((School?)null);

        CreateEmploymentSchoolDto createDto = new()
        {
            SchoolDisplayId = 999999999999L,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true
        };

        // Act
        Result<EmploymentSchoolResponseDto> result = await _employmentService.AddSchoolAssignmentAsync(employmentDisplayId, createDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    #endregion

    #region RemoveSchoolAssignmentAsync Tests

    [Fact]
    public async Task RemoveSchoolAssignmentAsync_WhenAssignmentExists_RemovesSuccessfully()
    {
        // Arrange
        long employmentSchoolDisplayId = 600000000001L;
        EmploymentSchool employmentSchool = new()
        {
            EmploymentId = Guid.NewGuid(),
            SchoolId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };
        SetDisplayId(employmentSchool, employmentSchoolDisplayId);

        _ = _employmentSchoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(employmentSchoolDisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employmentSchool);
        _ = _employmentSchoolRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _employmentService.RemoveSchoolAssignmentAsync(employmentSchoolDisplayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _employmentSchoolRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveSchoolAssignmentAsync_WhenAssignmentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _ = _employmentSchoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmploymentSchool?)null);

        // Act
        Result result = await _employmentService.RemoveSchoolAssignmentAsync(999999999999L, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    #endregion

    #region Helper Methods

    private static Employment CreateTestEmployment(
        long displayId,
        string personFirstName = "Test",
        string personLastName = "Person")
    {
        Person person = CreateTestPerson(100000000000L + (displayId % 1000), personFirstName, personLastName);
        Position position = CreateTestPosition(200000000001L);
        SalaryGrade salaryGrade = CreateTestSalaryGrade(300000000001L);
        Item item = CreateTestItem(400000000001L);

        Employment employment = new()
        {
            DepEdId = $"DEPED-{displayId}",
            AppointmentStatus = AppointmentStatus.Original,
            EmploymentStatus = EmploymentStatus.Regular,
            Eligibility = Eligibility.CivilServiceProfessional,
            IsActive = true,
            PersonId = person.Id,
            Person = person,
            PositionId = position.Id,
            Position = position,
            SalaryGradeId = salaryGrade.Id,
            SalaryGrade = salaryGrade,
            ItemId = item.Id,
            Item = item,
            EmploymentSchools = [],
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(employment, displayId);

        return employment;
    }

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

    private static Position CreateTestPosition(long displayId)
    {
        Position position = new()
        {
            TitleName = "Teacher I",
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
            SalaryGradeName = "SG-11",
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
            ItemName = "Item-001",
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(item, displayId);

        return item;
    }

    private static School CreateTestSchool(long displayId)
    {
        School school = new()
        {
            SchoolName = "Test Elementary School",
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(school, displayId);

        return school;
    }

    private static void SetDisplayId<T>(T entity, long displayId) where T : BaseEntity
    {
        PropertyInfo? displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(entity, displayId);
    }

    #endregion
}
