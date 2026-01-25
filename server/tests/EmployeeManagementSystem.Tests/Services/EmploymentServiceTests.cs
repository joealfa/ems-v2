using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;

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
        var displayId = 100000000001L;
        var employment = CreateTestEmployment(displayId);

        var employments = new List<Employment> { employment }.BuildMockQueryable();
        _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        var result = await _employmentService.GetByDisplayIdAsync(displayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(displayId, result.Value.DisplayId);
    }

    [Fact]
    public async Task GetByDisplayIdAsync_WhenEmploymentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;
        var employments = new List<Employment>().BuildMockQueryable();
        _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        var result = await _employmentService.GetByDisplayIdAsync(displayId);

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
        var employments = new List<Employment>
        {
            CreateTestEmployment(100000000001L),
            CreateTestEmployment(100000000002L),
            CreateTestEmployment(100000000003L)
        }.BuildMockQueryable();

        _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        var result = await _employmentService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        // Arrange
        var query = new PaginationQuery { PageNumber = 1, PageSize = 10, SearchTerm = "John" };
        var employments = new List<Employment>
        {
            CreateTestEmployment(100000000001L, personFirstName: "John", personLastName: "Doe"),
            CreateTestEmployment(100000000002L, personFirstName: "Jane", personLastName: "Smith"),
            CreateTestEmployment(100000000003L, personFirstName: "Bob", personLastName: "Johnson")
        }.BuildMockQueryable();

        _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        var result = await _employmentService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount); // John Doe and Bob Johnson
    }

    [Fact]
    public async Task GetPagedAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var query = new PaginationQuery { PageNumber = 2, PageSize = 2 };
        var employments = new List<Employment>
        {
            CreateTestEmployment(100000000001L, personFirstName: "Alice", personLastName: "Anderson"),
            CreateTestEmployment(100000000002L, personFirstName: "Bob", personLastName: "Brown"),
            CreateTestEmployment(100000000003L, personFirstName: "Charlie", personLastName: "Clark"),
            CreateTestEmployment(100000000004L, personFirstName: "David", personLastName: "Davis")
        }.BuildMockQueryable();

        _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        var result = await _employmentService.GetPagedAsync(query);

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
        var person = CreateTestPerson(100000000001L);
        var position = CreateTestPosition(200000000001L);
        var salaryGrade = CreateTestSalaryGrade(300000000001L);
        var item = CreateTestItem(400000000001L);

        _personRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(person.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);
        _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(position.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);
        _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(salaryGrade.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(salaryGrade);
        _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(item.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _employmentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employment e, CancellationToken _) => e);

        var createDto = new CreateEmploymentDto
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
        var result = await _employmentService.CreateAsync(createDto, "TestUser");

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
        _personRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person?)null);

        var createDto = new CreateEmploymentDto
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
        var result = await _employmentService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.BadRequest, result.FailureType);
    }

    [Fact]
    public async Task CreateAsync_WhenPositionNotFound_ReturnsBadRequest()
    {
        // Arrange
        var person = CreateTestPerson(100000000001L);

        _personRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(person.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);
        _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position?)null);

        var createDto = new CreateEmploymentDto
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
        var result = await _employmentService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.BadRequest, result.FailureType);
    }

    [Fact]
    public async Task CreateAsync_WithSchools_CreatesEmploymentWithSchoolAssignments()
    {
        // Arrange
        var person = CreateTestPerson(100000000001L);
        var position = CreateTestPosition(200000000001L);
        var salaryGrade = CreateTestSalaryGrade(300000000001L);
        var item = CreateTestItem(400000000001L);
        var school = CreateTestSchool(500000000001L);

        _personRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(person.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);
        _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(position.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);
        _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(salaryGrade.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(salaryGrade);
        _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(item.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        _schoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(school.DisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(school);

        _employmentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employment e, CancellationToken _) => e);
        _employmentSchoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmploymentSchool es, CancellationToken _) =>
            {
                // The service sets the School navigation property after adding to the collection
                es.School = school;
                return es;
            });

        var createDto = new CreateEmploymentDto
        {
            PersonDisplayId = person.DisplayId,
            PositionDisplayId = position.DisplayId,
            SalaryGradeDisplayId = salaryGrade.DisplayId,
            ItemDisplayId = item.DisplayId,
            AppointmentStatus = AppointmentStatus.Original,
            EmploymentStatus = EmploymentStatus.Regular,
            Eligibility = Eligibility.CivilServiceProfessional,
            Schools = new List<CreateEmploymentSchoolDto>
            {
                new CreateEmploymentSchoolDto
                {
                    SchoolDisplayId = school.DisplayId,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    IsCurrent = true
                }
            }
        };

        // Act
        var result = await _employmentService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Schools);
        _employmentSchoolRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenEmploymentExists_DeletesSuccessfully()
    {
        // Arrange
        var displayId = 100000000001L;
        var employment = CreateTestEmployment(displayId);
        employment.EmploymentSchools = new List<EmploymentSchool>();

        var employments = new List<Employment> { employment }.BuildMockQueryable();
        _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _employmentRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _employmentService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _employmentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenEmploymentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;
        var employments = new List<Employment>().BuildMockQueryable();
        _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);

        // Act
        var result = await _employmentService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    [Fact]
    public async Task DeleteAsync_WithSchoolAssignments_CascadeDeletesSchools()
    {
        // Arrange
        var displayId = 100000000001L;
        var employment = CreateTestEmployment(displayId);
        var employmentSchool = new EmploymentSchool
        {
            EmploymentId = employment.Id,
            SchoolId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };
        employment.EmploymentSchools = new List<EmploymentSchool> { employmentSchool };

        var employments = new List<Employment> { employment }.BuildMockQueryable();
        _employmentRepositoryMock.Setup(r => r.Query()).Returns(employments);
        _employmentRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<Employment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _employmentSchoolRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _employmentService.DeleteAsync(displayId, "TestUser");

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
        var employmentDisplayId = 100000000001L;
        var schoolDisplayId = 500000000001L;
        var employment = CreateTestEmployment(employmentDisplayId);
        var school = CreateTestSchool(schoolDisplayId);

        _employmentRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(employmentDisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employment);
        _schoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(schoolDisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(school);
        _employmentSchoolRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmploymentSchool es, CancellationToken _) => es);

        var createDto = new CreateEmploymentSchoolDto
        {
            SchoolDisplayId = schoolDisplayId,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true
        };

        // Act
        var result = await _employmentService.AddSchoolAssignmentAsync(employmentDisplayId, createDto, "TestUser");

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
        _employmentRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employment?)null);

        var createDto = new CreateEmploymentSchoolDto
        {
            SchoolDisplayId = 500000000001L,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true
        };

        // Act
        var result = await _employmentService.AddSchoolAssignmentAsync(999999999999L, createDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
    }

    [Fact]
    public async Task AddSchoolAssignmentAsync_WhenSchoolNotFound_ReturnsNotFound()
    {
        // Arrange
        var employmentDisplayId = 100000000001L;
        var employment = CreateTestEmployment(employmentDisplayId);

        _employmentRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(employmentDisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employment);
        _schoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((School?)null);

        var createDto = new CreateEmploymentSchoolDto
        {
            SchoolDisplayId = 999999999999L,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true
        };

        // Act
        var result = await _employmentService.AddSchoolAssignmentAsync(employmentDisplayId, createDto, "TestUser");

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
        var employmentSchoolDisplayId = 600000000001L;
        var employmentSchool = new EmploymentSchool
        {
            EmploymentId = Guid.NewGuid(),
            SchoolId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsCurrent = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };
        SetDisplayId(employmentSchool, employmentSchoolDisplayId);

        _employmentSchoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(employmentSchoolDisplayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employmentSchool);
        _employmentSchoolRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _employmentService.RemoveSchoolAssignmentAsync(employmentSchoolDisplayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _employmentSchoolRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<EmploymentSchool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveSchoolAssignmentAsync_WhenAssignmentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _employmentSchoolRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmploymentSchool?)null);

        // Act
        var result = await _employmentService.RemoveSchoolAssignmentAsync(999999999999L, "TestUser");

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
        var person = CreateTestPerson(100000000000L + displayId % 1000, personFirstName, personLastName);
        var position = CreateTestPosition(200000000001L);
        var salaryGrade = CreateTestSalaryGrade(300000000001L);
        var item = CreateTestItem(400000000001L);

        var employment = new Employment
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
            EmploymentSchools = new List<EmploymentSchool>(),
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        SetDisplayId(employment, displayId);

        return employment;
    }

    private static Person CreateTestPerson(long displayId, string firstName = "Test", string lastName = "Person")
    {
        var person = new Person
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
        var position = new Position
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
        var salaryGrade = new SalaryGrade
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
        var item = new Item
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
        var school = new School
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
        var displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(entity, displayId);
    }

    #endregion
}
