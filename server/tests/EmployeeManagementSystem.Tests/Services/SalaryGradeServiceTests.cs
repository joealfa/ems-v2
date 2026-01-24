using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.SalaryGrade;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;

namespace EmployeeManagementSystem.Tests.Services;

public class SalaryGradeServiceTests
{
    private readonly Mock<IRepository<SalaryGrade>> _salaryGradeRepositoryMock;
    private readonly SalaryGradeService _salaryGradeService;

    public SalaryGradeServiceTests()
    {
        _salaryGradeRepositoryMock = new Mock<IRepository<SalaryGrade>>();
        _salaryGradeService = new SalaryGradeService(_salaryGradeRepositoryMock.Object);
    }

    #region GetByDisplayIdAsync Tests

    [Fact]
    public async Task GetByDisplayIdAsync_WhenSalaryGradeExists_ReturnsSalaryGradeResponseDto()
    {
        // Arrange
        var displayId = 123456789012L;
        var salaryGrade = CreateTestSalaryGrade(displayId, "SG-11", 1, 27000m);

        _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(salaryGrade);

        // Act
        var result = await _salaryGradeService.GetByDisplayIdAsync(displayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(displayId, result.Value.DisplayId);
        Assert.Equal(salaryGrade.SalaryGradeName, result.Value.SalaryGradeName);
        Assert.Equal(salaryGrade.Step, result.Value.Step);
        Assert.Equal(salaryGrade.MonthlySalary, result.Value.MonthlySalary);
    }

    [Fact]
    public async Task GetByDisplayIdAsync_WhenSalaryGradeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;

        _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SalaryGrade?)null);

        // Act
        var result = await _salaryGradeService.GetByDisplayIdAsync(displayId);

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
        var salaryGrades = new List<SalaryGrade>
        {
            CreateTestSalaryGrade(100000000001L, "SG-10", 1, 25000m),
            CreateTestSalaryGrade(100000000002L, "SG-11", 1, 27000m),
            CreateTestSalaryGrade(100000000003L, "SG-12", 1, 29000m)
        }.BuildMockQueryable();

        _salaryGradeRepositoryMock.Setup(r => r.Query()).Returns(salaryGrades);

        // Act
        var result = await _salaryGradeService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        // Arrange
        var query = new PaginationQuery { PageNumber = 1, PageSize = 10, SearchTerm = "SG-11" };
        var salaryGrades = new List<SalaryGrade>
        {
            CreateTestSalaryGrade(100000000001L, "SG-10", 1, 25000m),
            CreateTestSalaryGrade(100000000002L, "SG-11", 1, 27000m),
            CreateTestSalaryGrade(100000000003L, "SG-12", 1, 29000m)
        }.BuildMockQueryable();

        _salaryGradeRepositoryMock.Setup(r => r.Query()).Returns(salaryGrades);

        // Act
        var result = await _salaryGradeService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetPagedAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var query = new PaginationQuery { PageNumber = 2, PageSize = 2 };
        var salaryGrades = new List<SalaryGrade>
        {
            CreateTestSalaryGrade(100000000001L, "SG-10", 1, 25000m),
            CreateTestSalaryGrade(100000000002L, "SG-11", 1, 27000m),
            CreateTestSalaryGrade(100000000003L, "SG-12", 1, 29000m),
            CreateTestSalaryGrade(100000000004L, "SG-13", 1, 31000m)
        }.BuildMockQueryable();

        _salaryGradeRepositoryMock.Setup(r => r.Query()).Returns(salaryGrades);

        // Act
        var result = await _salaryGradeService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.PageNumber);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedSalaryGrade()
    {
        // Arrange
        var createDto = new CreateSalaryGradeDto
        {
            SalaryGradeName = "SG-15",
            Description = "Test salary grade",
            Step = 1,
            MonthlySalary = 35000m
        };

        _salaryGradeRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SalaryGrade>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SalaryGrade sg, CancellationToken _) => sg);

        // Act
        var result = await _salaryGradeService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createDto.SalaryGradeName, result.Value.SalaryGradeName);
        Assert.Equal(createDto.Description, result.Value.Description);
        Assert.Equal(createDto.Step, result.Value.Step);
        Assert.Equal(createDto.MonthlySalary, result.Value.MonthlySalary);
        Assert.True(result.Value.IsActive);

        _salaryGradeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SalaryGrade>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDifferentSteps_CreatesCorrectly()
    {
        // Arrange
        var createDto = new CreateSalaryGradeDto
        {
            SalaryGradeName = "SG-15",
            Step = 5,
            MonthlySalary = 40000m
        };

        _salaryGradeRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SalaryGrade>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SalaryGrade sg, CancellationToken _) => sg);

        // Act
        var result = await _salaryGradeService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(5, result.Value.Step);
        Assert.Equal(40000m, result.Value.MonthlySalary);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenSalaryGradeExists_ReturnsUpdatedSalaryGrade()
    {
        // Arrange
        var displayId = 123456789012L;
        var existingSalaryGrade = CreateTestSalaryGrade(displayId, "SG-11", 1, 27000m);
        var updateDto = new UpdateSalaryGradeDto
        {
            SalaryGradeName = "SG-11",
            Description = "Updated Description",
            Step = 2,
            MonthlySalary = 28000m,
            IsActive = true
        };

        _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSalaryGrade);

        _salaryGradeRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<SalaryGrade>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _salaryGradeService.UpdateAsync(displayId, updateDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(updateDto.Step, result.Value.Step);
        Assert.Equal(updateDto.MonthlySalary, result.Value.MonthlySalary);

        _salaryGradeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SalaryGrade>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenSalaryGradeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;
        var updateDto = new UpdateSalaryGradeDto
        {
            SalaryGradeName = "SG-99",
            Step = 1,
            MonthlySalary = 50000m
        };

        _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SalaryGrade?)null);

        // Act
        var result = await _salaryGradeService.UpdateAsync(displayId, updateDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _salaryGradeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SalaryGrade>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenSalaryGradeExists_ReturnsTrue()
    {
        // Arrange
        var displayId = 123456789012L;
        var salaryGrade = CreateTestSalaryGrade(displayId, "SG-11", 1, 27000m);

        _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(salaryGrade);

        _salaryGradeRepositoryMock
            .Setup(r => r.DeleteAsync(salaryGrade, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _salaryGradeService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _salaryGradeRepositoryMock.Verify(r => r.DeleteAsync(salaryGrade, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenSalaryGradeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;

        _salaryGradeRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SalaryGrade?)null);

        // Act
        var result = await _salaryGradeService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _salaryGradeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<SalaryGrade>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static SalaryGrade CreateTestSalaryGrade(long displayId, string salaryGradeName, int step, decimal monthlySalary, string? description = null)
    {
        var salaryGrade = new SalaryGrade
        {
            SalaryGradeName = salaryGradeName,
            Description = description,
            Step = step,
            MonthlySalary = monthlySalary,
            IsActive = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        // Use reflection to set DisplayId since it has a private setter
        var displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(salaryGrade, displayId);

        return salaryGrade;
    }

    #endregion
}
