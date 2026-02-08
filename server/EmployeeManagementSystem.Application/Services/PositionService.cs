using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Position;
using EmployeeManagementSystem.Application.Events;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Mappings;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Events.Positions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for position operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PositionService"/> class.
/// </remarks>
public class PositionService(
    IRepository<Position> positionRepository,
    IEventPublisher eventPublisher,
    IHttpContextAccessor httpContextAccessor) : IPositionService
{
    private readonly IRepository<Position> _positionRepository = positionRepository;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <inheritdoc />
    public async Task<Result<PositionResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        Position? position = await _positionRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        return position == null
            ? Result<PositionResponseDto>.NotFound("Position not found.")
            : Result<PositionResponseDto>.Success(position.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<PagedResult<PositionResponseDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Position> queryable = _positionRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            string searchTerm = query.SearchTerm.ToLower();
            queryable = queryable.Where(p =>
                p.TitleName.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        int totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(p => p.TitleName)
            : queryable.OrderBy(p => p.TitleName);

        List<PositionResponseDto> items = await queryable
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
        Position position = new()
        {
            TitleName = dto.TitleName,
            Description = dto.Description,
            CreatedBy = createdBy
        };

        _ = await _positionRepository.AddAsync(position, cancellationToken);

        // Publish domain event
        await PublishPositionCreatedEventAsync(position, createdBy, cancellationToken);

        return Result<PositionResponseDto>.Success(position.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result<PositionResponseDto>> UpdateAsync(long displayId, UpdatePositionDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        Position? position = await _positionRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (position == null)
        {
            return Result<PositionResponseDto>.NotFound("Position not found.");
        }

        position.TitleName = dto.TitleName;
        position.Description = dto.Description;
        position.IsActive = dto.IsActive;
        position.ModifiedBy = modifiedBy;

        await _positionRepository.UpdateAsync(position, cancellationToken);

        // Publish domain event
        Dictionary<string, object?> changes = new()
        {
            ["TitleName"] = dto.TitleName,
            ["Description"] = dto.Description,
            ["IsActive"] = dto.IsActive
        };
        await PublishPositionUpdatedEventAsync(position, changes, modifiedBy, cancellationToken);

        return Result<PositionResponseDto>.Success(position.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        Position? position = await _positionRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (position == null)
        {
            return Result.NotFound("Position not found.");
        }

        position.ModifiedBy = deletedBy;
        await _positionRepository.DeleteAsync(position, cancellationToken);

        // Publish domain event
        await PublishPositionDeletedEventAsync(position, deletedBy, cancellationToken);

        return Result.Success();
    }

    #region Event Publishing Helpers

    private async Task PublishPositionCreatedEventAsync(Position position, string userId, CancellationToken cancellationToken)
    {
        try
        {
            PositionCreatedEvent domainEvent = new(
                positionId: position.Id,
                titleName: position.TitleName,
                description: position.Description,
                isActive: position.IsActive
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
            Console.WriteLine($"Failed to publish PositionCreatedEvent: {ex.Message}");
        }
    }

    private async Task PublishPositionUpdatedEventAsync(
        Position position,
        Dictionary<string, object?> changes,
        string userId,
        CancellationToken cancellationToken)
    {
        try
        {
            PositionUpdatedEvent domainEvent = new(position.Id, changes);
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
            Console.WriteLine($"Failed to publish PositionUpdatedEvent: {ex.Message}");
        }
    }

    private async Task PublishPositionDeletedEventAsync(Position position, string userId, CancellationToken cancellationToken)
    {
        try
        {
            PositionDeletedEvent domainEvent = new(position.Id, position.TitleName);
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
            Console.WriteLine($"Failed to publish PositionDeletedEvent: {ex.Message}");
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
                Source = "PositionService"
            };
    }

    #endregion
}
