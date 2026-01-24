using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Item;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Tests.Helpers;
using Moq;

namespace EmployeeManagementSystem.Tests.Services;

public class ItemServiceTests
{
    private readonly Mock<IRepository<Item>> _itemRepositoryMock;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _itemRepositoryMock = new Mock<IRepository<Item>>();
        _itemService = new ItemService(_itemRepositoryMock.Object);
    }

    #region GetByDisplayIdAsync Tests

    [Fact]
    public async Task GetByDisplayIdAsync_WhenItemExists_ReturnsItemResponseDto()
    {
        // Arrange
        var displayId = 123456789012L;
        var item = CreateTestItem(displayId, "Teacher I", "Entry level position");

        _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _itemService.GetByDisplayIdAsync(displayId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(displayId, result.Value.DisplayId);
        Assert.Equal(item.ItemName, result.Value.ItemName);
        Assert.Equal(item.Description, result.Value.Description);
    }

    [Fact]
    public async Task GetByDisplayIdAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;

        _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        // Act
        var result = await _itemService.GetByDisplayIdAsync(displayId);

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
        var items = new List<Item>
        {
            CreateTestItem(100000000001L, "Item A"),
            CreateTestItem(100000000002L, "Item B"),
            CreateTestItem(100000000003L, "Item C")
        }.BuildMockQueryable();

        _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        var result = await _itemService.GetPagedAsync(query);

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
        var items = new List<Item>
        {
            CreateTestItem(100000000001L, "Teacher I"),
            CreateTestItem(100000000002L, "Principal"),
            CreateTestItem(100000000003L, "Teacher II")
        }.BuildMockQueryable();

        _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        var result = await _itemService.GetPagedAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedItem()
    {
        // Arrange
        var createDto = new CreateItemDto
        {
            ItemName = "New Item",
            Description = "Test description"
        };

        _itemRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item i, CancellationToken _) => i);

        // Act
        var result = await _itemService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createDto.ItemName, result.Value.ItemName);
        Assert.Equal(createDto.Description, result.Value.Description);
        Assert.True(result.Value.IsActive);

        _itemRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithoutDescription_CreatesItemSuccessfully()
    {
        // Arrange
        var createDto = new CreateItemDto
        {
            ItemName = "New Item"
        };

        _itemRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item i, CancellationToken _) => i);

        // Act
        var result = await _itemService.CreateAsync(createDto, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(createDto.ItemName, result.Value.ItemName);
        Assert.Null(result.Value.Description);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsSuccess()
    {
        // Arrange
        var displayId = 123456789012L;
        var item = CreateTestItem(displayId, "Test Item");

        _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _itemRepositoryMock
            .Setup(r => r.DeleteAsync(item, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _itemService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _itemRepositoryMock.Verify(r => r.DeleteAsync(item, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var displayId = 999999999999L;

        _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        // Act
        var result = await _itemService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _itemRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static Item CreateTestItem(long displayId, string itemName, string? description = null)
    {
        var item = new Item
        {
            ItemName = itemName,
            Description = description,
            IsActive = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        // Use reflection to set DisplayId since it has a private setter
        var displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(item, displayId);

        return item;
    }

    #endregion
}
