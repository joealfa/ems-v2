using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Item;
using EmployeeManagementSystem.Application.Events;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Mappings;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Events.Items;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for item operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ItemService"/> class.
/// </remarks>
public class ItemService(
    IRepository<Item> itemRepository,
    IEventPublisher eventPublisher,
    IHttpContextAccessor httpContextAccessor) : IItemService
{
    private readonly IRepository<Item> _itemRepository = itemRepository;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <inheritdoc />
    public async Task<Result<ItemResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        Item? item = await _itemRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        return item == null
            ? Result<ItemResponseDto>.NotFound($"Item with ID {displayId} not found.")
            : Result<ItemResponseDto>.Success(item.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<PagedResult<ItemResponseDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Item> queryable = _itemRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            string searchTerm = query.SearchTerm.ToLower();
            queryable = queryable.Where(i =>
                i.ItemName.ToLower().Contains(searchTerm) ||
                (i.Description != null && i.Description.ToLower().Contains(searchTerm)));
        }

        int totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(i => i.ItemName)
            : queryable.OrderBy(i => i.ItemName);

        List<ItemResponseDto> items = await queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new ItemResponseDto
            {
                DisplayId = i.DisplayId,
                ItemName = i.ItemName,
                Description = i.Description,
                IsActive = i.IsActive,
                CreatedOn = i.CreatedOn,
                CreatedBy = i.CreatedBy,
                ModifiedOn = i.ModifiedOn,
                ModifiedBy = i.ModifiedBy
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ItemResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<Result<ItemResponseDto>> CreateAsync(CreateItemDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        Item item = new()
        {
            ItemName = dto.ItemName,
            Description = dto.Description,
            CreatedBy = createdBy
        };

        _ = await _itemRepository.AddAsync(item, cancellationToken);

        // Publish domain event
        await PublishItemCreatedEventAsync(item, createdBy, cancellationToken);

        return Result<ItemResponseDto>.Success(item.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result<ItemResponseDto>> UpdateAsync(long displayId, UpdateItemDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        Item? item = await _itemRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (item == null)
        {
            return Result<ItemResponseDto>.NotFound($"Item with ID {displayId} not found.");
        }

        item.ItemName = dto.ItemName;
        item.Description = dto.Description;
        item.IsActive = dto.IsActive;
        item.ModifiedBy = modifiedBy;

        await _itemRepository.UpdateAsync(item, cancellationToken);

        // Publish domain event
        Dictionary<string, object?> changes = new()
        {
            ["ItemName"] = dto.ItemName,
            ["Description"] = dto.Description,
            ["IsActive"] = dto.IsActive
        };
        await PublishItemUpdatedEventAsync(item, changes, modifiedBy, cancellationToken);

        return Result<ItemResponseDto>.Success(item.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        Item? item = await _itemRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (item == null)
        {
            return Result.NotFound($"Item with ID {displayId} not found.");
        }

        item.ModifiedBy = deletedBy;
        await _itemRepository.DeleteAsync(item, cancellationToken);

        // Publish domain event
        await PublishItemDeletedEventAsync(item, deletedBy, cancellationToken);

        return Result.Success();
    }

    #region Event Publishing Helpers

    private async Task PublishItemCreatedEventAsync(Item item, string userId, CancellationToken cancellationToken)
    {
        try
        {
            ItemCreatedEvent domainEvent = new(
                itemId: item.Id,
                itemName: item.ItemName,
                description: item.Description,
                isActive: item.IsActive
            );

            EventMetadata metadata = CreateEventMetadata();

            await _eventPublisher.PublishAsync(
                domainEvent,
                userId,
                _httpContextAccessor.HttpContext?.TraceIdentifier,
                metadata,
                cancellationToken);
        }
        catch (Exception ex)
        {
            // Log but don't throw - event publishing failures should not fail the main operation
            Console.WriteLine($"Failed to publish ItemCreatedEvent: {ex.Message}");
        }
    }

    private async Task PublishItemUpdatedEventAsync(
        Item item,
        Dictionary<string, object?> changes,
        string userId,
        CancellationToken cancellationToken)
    {
        try
        {
            ItemUpdatedEvent domainEvent = new(item.Id, changes);
            EventMetadata metadata = CreateEventMetadata();

            await _eventPublisher.PublishAsync(
                domainEvent,
                userId,
                _httpContextAccessor.HttpContext?.TraceIdentifier,
                metadata,
                cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to publish ItemUpdatedEvent: {ex.Message}");
        }
    }

    private async Task PublishItemDeletedEventAsync(Item item, string userId, CancellationToken cancellationToken)
    {
        try
        {
            ItemDeletedEvent domainEvent = new(item.Id, item.ItemName);
            EventMetadata metadata = CreateEventMetadata();

            await _eventPublisher.PublishAsync(
                domainEvent,
                userId,
                _httpContextAccessor.HttpContext?.TraceIdentifier,
                metadata,
                cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to publish ItemDeletedEvent: {ex.Message}");
        }
    }

    private EventMetadata CreateEventMetadata()
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext;
        return httpContext == null
            ? new EventMetadata()
            : new EventMetadata
            {
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                Source = "ItemService"
            };
    }

    #endregion
}
