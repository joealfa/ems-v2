using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Position;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;

namespace EmployeeManagementSystem.Tests.Services;

public class PositionServiceTests
{
    private readonly Mock<IRepository<Position>> _positionRepositoryMock;
    private readonly PositionService _positionService;

    public PositionServiceTests()
    {
        _positionRepositoryMock = new Mock<IRepository<Position>>();
        _positionService = new PositionService(_positionRepositoryMock.Object);
    }

    #region GetByDisplayIdAsync Tests

    [Fact]
    public async Task GetByDisplayIdAsync_WhenPositionExists_ReturnsPositionResponseDto()
    {
        // Arrange
        var displayId = 123456789012L;
        var position = CreateTestPosition(displayId, "Teacher I", "Entry level teaching position");

        _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);

        // Act
        var result = await _positionService.GetByDisplayIdAsync(displayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(displayId, result.Value.DisplayId);
        Assert.Equal(position.TitleName, result.Value.TitleName);
        Assert.Equal(position.Description, result.Value.Description);
    }

    [Fact]
    public async Task GetByDisplayIdAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;

        _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position?)null);

        // Act
        var result = await _positionService.GetByDisplayIdAsync(displayId);

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
        var positions = new List<Position>
        {
            CreateTestPosition(100000000001L, "Position A"),
            CreateTestPosition(100000000002L, "Position B"),
            CreateTestPosition(100000000003L, "Position C")
        }.BuildMockQueryable();

        _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);

        // Act
        var result = await _positionService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        // Arrange
        var query = new PaginationQuery { PageNumber = 1, PageSize = 10, SearchTerm = "Teacher" };
        var positions = new List<Position>
        {
            CreateTestPosition(100000000001L, "Teacher I"),
            CreateTestPosition(100000000002L, "Principal"),
            CreateTestPosition(100000000003L, "Teacher III")
        }.BuildMockQueryable();

        _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);

        // Act
        var result = await _positionService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_EmptyDatabase_ReturnsEmptyResult()
    {
        // Arrange
        var query = new PaginationQuery { PageNumber = 1, PageSize = 10 };
        var positions = new List<Position>().BuildMockQueryable();

        _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);

        // Act
        var result = await _positionService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedPosition()
    {
        // Arrange
        var createDto = new CreatePositionDto
        {
            TitleName = "New Position",
            Description = "Test position description"
        };

        _positionRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position p, CancellationToken _) => p);

        // Act
        var result = await _positionService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createDto.TitleName, result.Value.TitleName);
        Assert.Equal(createDto.Description, result.Value.Description);
        Assert.True(result.Value.IsActive);

        _positionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithoutDescription_CreatesPositionSuccessfully()
    {
        // Arrange
        var createDto = new CreatePositionDto
        {
            TitleName = "New Position"
        };

        _positionRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position p, CancellationToken _) => p);

        // Act
        var result = await _positionService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createDto.TitleName, result.Value.TitleName);
        Assert.Null(result.Value.Description);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenPositionExists_ReturnsUpdatedPosition()
    {
        // Arrange
        var displayId = 123456789012L;
        var existingPosition = CreateTestPosition(displayId, "Original Title", "Original Description");
        var updateDto = new UpdatePositionDto
        {
            TitleName = "Updated Title",
            Description = "Updated Description",
            IsActive = true
        };

        _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPosition);

        _positionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _positionService.UpdateAsync(displayId, updateDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(updateDto.TitleName, result.Value.TitleName);
        Assert.Equal(updateDto.Description, result.Value.Description);

        _positionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;
        var updateDto = new UpdatePositionDto
        {
            TitleName = "Updated Title"
        };

        _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position?)null);

        // Act
        var result = await _positionService.UpdateAsync(displayId, updateDto, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _positionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_ReturnsTrue()
    {
        // Arrange
        var displayId = 123456789012L;
        var position = CreateTestPosition(displayId, "Test Position");

        _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);

        _positionRepositoryMock
            .Setup(r => r.DeleteAsync(position, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _positionService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _positionRepositoryMock.Verify(r => r.DeleteAsync(position, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;

        _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position?)null);

        // Act
        var result = await _positionService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _positionRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static Position CreateTestPosition(long displayId, string titleName, string? description = null)
    {
        var position = new Position
        {
            TitleName = titleName,
            Description = description,
            IsActive = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        // Use reflection to set DisplayId since it has a private setter
        var displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(position, displayId);

        return position;
    }

    #endregion
}
