using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Item;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Services;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Reflection;

namespace EmployeeManagementSystem.Tests.Services;

public class ItemServiceTests
{
    private readonly Mock<IRepository<Item>> _itemRepositoryMock;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _itemRepositoryMock = new Mock<IRepository<Item>>();
        Mock<IEventPublisher> eventPublisherMock = new();
        Mock<IHttpContextAccessor> httpContextAccessorMock = new();
        _itemService = new ItemService(
            _itemRepositoryMock.Object,
            eventPublisherMock.Object,
            httpContextAccessorMock.Object);
    }

    #region GetByDisplayIdAsync Tests

    [Fact]
    public async Task GetByDisplayIdAsync_WhenItemExists_ReturnsItemResponseDto()
    {
        // Arrange
        long displayId = 123456789012L;
        Item item = CreateTestItem(displayId, "Teacher I", "Entry level position");

        _ = _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        Result<ItemResponseDto> result = await _itemService.GetByDisplayIdAsync(displayId);

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
        long displayId = 999999999999L;

        _ = _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        // Act
        Result<ItemResponseDto> result = await _itemService.GetByDisplayIdAsync(displayId);

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
        IQueryable<Item> items = new List<Item>
        {
            CreateTestItem(100000000001L, "Item A"),
            CreateTestItem(100000000002L, "Item B"),
            CreateTestItem(100000000003L, "Item C")
        }.BuildMockQueryable();

        _ = _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        PagedResult<ItemResponseDto> result = await _itemService.GetPagedAsync(query);

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
        IQueryable<Item> items = new List<Item>
        {
            CreateTestItem(100000000001L, "Teacher I"),
            CreateTestItem(100000000002L, "Principal"),
            CreateTestItem(100000000003L, "Teacher II")
        }.BuildMockQueryable();

        _ = _itemRepositoryMock.Setup(r => r.Query()).Returns(items);

        // Act
        PagedResult<ItemResponseDto> result = await _itemService.GetPagedAsync(query);

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
        CreateItemDto createDto = new()
        {
            ItemName = "New Item",
            Description = "Test description"
        };

        _ = _itemRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item i, CancellationToken _) => i);

        // Act
        Result<ItemResponseDto> result = await _itemService.CreateAsync(createDto, "TestUser");

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
        CreateItemDto createDto = new()
        {
            ItemName = "New Item"
        };

        _ = _itemRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item i, CancellationToken _) => i);

        // Act
        Result<ItemResponseDto> result = await _itemService.CreateAsync(createDto, "TestUser");

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
        long displayId = 123456789012L;
        Item item = CreateTestItem(displayId, "Test Item");

        _ = _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _ = _itemRepositoryMock
            .Setup(r => r.DeleteAsync(item, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _itemService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.True(result.IsSuccess);
        _itemRepositoryMock.Verify(r => r.DeleteAsync(item, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        long displayId = 999999999999L;

        _ = _itemRepositoryMock
            .Setup(r => r.GetByDisplayIdAsync(displayId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        // Act
        Result result = await _itemService.DeleteAsync(displayId, "TestUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(FailureType.NotFound, result.FailureType);
        _itemRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static Item CreateTestItem(long displayId, string itemName, string? description = null)
    {
        Item item = new()
        {
            ItemName = itemName,
            Description = description,
            IsActive = true,
            CreatedBy = "System",
            CreatedOn = DateTime.UtcNow
        };

        // Use reflection to set DisplayId since it has a private setter
        PropertyInfo? displayIdProperty = typeof(BaseEntity).GetProperty("DisplayId");
        displayIdProperty?.SetValue(item, displayId);

        return item;
    }

    #endregion
}
