using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Item;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for item operations.
/// </summary>
public class ItemService : IItemService
{
    private readonly IRepository<Item> _itemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemService"/> class.
    /// </summary>
    public ItemService(IRepository<Item> itemRepository)
    {
        _itemRepository = itemRepository;
    }

    /// <inheritdoc />
    public async Task<ItemResponseDto?> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        return item == null ? null : MapToResponseDto(item);
    }

    /// <inheritdoc />
    public async Task<PagedResult<ItemResponseDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _itemRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.ToLower();
            queryable = queryable.Where(i =>
                i.ItemName.ToLower().Contains(searchTerm) ||
                (i.Description != null && i.Description.ToLower().Contains(searchTerm)));
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(i => i.ItemName)
            : queryable.OrderBy(i => i.ItemName);

        var items = await queryable
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
    public async Task<ItemResponseDto> CreateAsync(CreateItemDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var item = new Item
        {
            ItemName = dto.ItemName,
            Description = dto.Description,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };

        await _itemRepository.AddAsync(item, cancellationToken);

        return MapToResponseDto(item);
    }

    /// <inheritdoc />
    public async Task<ItemResponseDto?> UpdateAsync(long displayId, UpdateItemDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (item == null)
            return null;

        item.ItemName = dto.ItemName;
        item.Description = dto.Description;
        item.IsActive = dto.IsActive;
        item.ModifiedBy = modifiedBy;
        item.ModifiedOn = DateTime.UtcNow;

        await _itemRepository.UpdateAsync(item, cancellationToken);

        return MapToResponseDto(item);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (item == null)
            return false;

        item.ModifiedBy = deletedBy;
        item.ModifiedOn = DateTime.UtcNow;
        await _itemRepository.DeleteAsync(item, cancellationToken);
        return true;
    }

    private static ItemResponseDto MapToResponseDto(Item item)
    {
        return new ItemResponseDto
        {
            DisplayId = item.DisplayId,
            ItemName = item.ItemName,
            Description = item.Description,
            IsActive = item.IsActive,
            CreatedOn = item.CreatedOn,
            CreatedBy = item.CreatedBy,
            ModifiedOn = item.ModifiedOn,
            ModifiedBy = item.ModifiedBy
        };
    }
}
