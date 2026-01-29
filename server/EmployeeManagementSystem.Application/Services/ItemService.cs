using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Item;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Mappings;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for item operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ItemService"/> class.
/// </remarks>
public class ItemService(IRepository<Item> itemRepository) : IItemService
{
    private readonly IRepository<Item> _itemRepository = itemRepository;

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

        return Result.Success();
    }
}
