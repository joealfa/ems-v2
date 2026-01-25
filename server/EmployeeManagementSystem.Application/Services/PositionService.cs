using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Position;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for position operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PositionService"/> class.
/// </remarks>
public class PositionService(IRepository<Position> positionRepository) : IPositionService
{
    private readonly IRepository<Position> _positionRepository = positionRepository;

    /// <inheritdoc />
    public async Task<Result<PositionResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        return position == null ? Result<PositionResponseDto>.NotFound("Position not found.") : Result<PositionResponseDto>.Success(MapToResponseDto(position));
    }

    /// <inheritdoc />
    public async Task<PagedResult<PositionResponseDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var queryable = _positionRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.ToLower();
            queryable = queryable.Where(p =>
                p.TitleName.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(p => p.TitleName)
            : queryable.OrderBy(p => p.TitleName);

        var items = await queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new PositionResponseDto
            {
                DisplayId = p.DisplayId,
                TitleName = p.TitleName,
                Description = p.Description,
                IsActive = p.IsActive,
                CreatedOn = p.CreatedOn,
                CreatedBy = p.CreatedBy,
                ModifiedOn = p.ModifiedOn,
                ModifiedBy = p.ModifiedBy
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PositionResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<Result<PositionResponseDto>> CreateAsync(CreatePositionDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var position = new Position
        {
            TitleName = dto.TitleName,
            Description = dto.Description,
            CreatedBy = createdBy
        };

        await _positionRepository.AddAsync(position, cancellationToken);

        return Result<PositionResponseDto>.Success(MapToResponseDto(position));
    }

    /// <inheritdoc />
    public async Task<Result<PositionResponseDto>> UpdateAsync(long displayId, UpdatePositionDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (position == null)
            return Result<PositionResponseDto>.NotFound("Position not found.");

        position.TitleName = dto.TitleName;
        position.Description = dto.Description;
        position.IsActive = dto.IsActive;
        position.ModifiedBy = modifiedBy;

        await _positionRepository.UpdateAsync(position, cancellationToken);

        return Result<PositionResponseDto>.Success(MapToResponseDto(position));
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        var position = await _positionRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (position == null)
            return Result.NotFound("Position not found.");

        position.ModifiedBy = deletedBy;
        await _positionRepository.DeleteAsync(position, cancellationToken);
        return Result.Success();
    }

    private static PositionResponseDto MapToResponseDto(Position position)
    {
        return new PositionResponseDto
        {
            DisplayId = position.DisplayId,
            TitleName = position.TitleName,
            Description = position.Description,
            IsActive = position.IsActive,
            CreatedOn = position.CreatedOn,
            CreatedBy = position.CreatedBy,
            ModifiedOn = position.ModifiedOn,
            ModifiedBy = position.ModifiedBy
        };
    }
}
