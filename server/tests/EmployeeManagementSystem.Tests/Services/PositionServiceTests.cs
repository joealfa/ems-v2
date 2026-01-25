using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Position;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;
using System.Reflection;

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
        long displayId = 123456789012L;
        Position position = CreateTestPosition(displayId, "Teacher I", "Entry level teaching position");

        _ = _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);

        // Act
        Result<PositionResponseDto> result = await _positionService.GetByDisplayIdAsync(displayId);

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
        long displayId = 999999999999L;

        _ = _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position?)null);

        // Act
        Result<PositionResponseDto> result = await _positionService.GetByDisplayIdAsync(displayId);

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
        IQueryable<Position> positions = new List<Position>
        {
            CreateTestPosition(100000000001L, "Position A"),
            CreateTestPosition(100000000002L, "Position B"),
            CreateTestPosition(100000000003L, "Position C")
        }.BuildMockQueryable();

        _ = _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);

        // Act
        PagedResult<PositionResponseDto> result = await _positionService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        // Arrange
        PaginationQuery query = new() { PageNumber = 1, PageSize = 10, SearchTerm = "Teacher" };
        IQueryable<Position> positions = new List<Position>
        {
            CreateTestPosition(100000000001L, "Teacher I"),
            CreateTestPosition(100000000002L, "Principal"),
            CreateTestPosition(100000000003L, "Teacher III")
        }.BuildMockQueryable();

        _ = _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);

        // Act
        PagedResult<PositionResponseDto> result = await _positionService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_EmptyDatabase_ReturnsEmptyResult()
    {
        // Arrange
        PaginationQuery query = new() { PageNumber = 1, PageSize = 10 };
        IQueryable<Position> positions = new List<Position>().BuildMockQueryable();

        _ = _positionRepositoryMock.Setup(r => r.Query()).Returns(positions);

        // Act
        PagedResult<PositionResponseDto> result = await _positionService.GetPagedAsync(query);

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
        CreatePositionDto createDto = new()
        {
            TitleName = "New Position",
            Description = "Test position description"
        };

        _ = _positionRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position p, CancellationToken _) => p);

        // Act
        Result<PositionResponseDto> result = await _positionService.CreateAsync(createDto, "TestUser");

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
        CreatePositionDto createDto = new()
        {
            TitleName = "New Position"
        };

        _ = _positionRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position p, CancellationToken _) => p);

        // Act
        Result<PositionResponseDto> result = await _positionService.CreateAsync(createDto, "TestUser");

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
        long displayId = 123456789012L;
        Position existingPosition = CreateTestPosition(displayId, "Original Title", "Original Description");
        UpdatePositionDto updateDto = new()
        {
            TitleName = "Updated Title",
            Description = "Updated Description",
            IsActive = true
        };

        _ = _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPosition);

        _ = _positionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result<PositionResponseDto> result = await _positionService.UpdateAsync(displayId, updateDto, "TestUser");

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
        long displayId = 999999999999L;
        UpdatePositionDto updateDto = new()
        {
            TitleName = "Updated Title"
        };

        _ = _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position?)null);

        // Act
        Result<PositionResponseDto> result = await _positionService.UpdateAsync(displayId, updateDto, "TestUser");

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
        long displayId = 123456789012L;
        Position position = CreateTestPosition(displayId, "Test Position");

        _ = _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(position);

        _ = _positionRepositoryMock
            .Setup(r => r.DeleteAsync(position, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _positionService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _positionRepositoryMock.Verify(r => r.DeleteAsync(position, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long displayId = 999999999999L;

        _ = _positionRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Position?)null);

        // Act
        Result result = await _positionService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _positionRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static Position CreateTestPosition(long displayId, string titleName, string? description = null)
    {
        Position position = new()
        {
            TitleName = titleName,
            Description = description,
            IsActive = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        // Use reflection to set DisplayId since it has a private setter
        PropertyInfo? displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(position, displayId);

        return position;
    }

    #endregion
}
