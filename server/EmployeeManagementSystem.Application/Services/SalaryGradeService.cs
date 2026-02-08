using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.SalaryGrade;
using EmployeeManagementSystem.Application.Events;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Application.Mappings;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Events.SalaryGrades;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for salary grade operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SalaryGradeService"/> class.
/// </remarks>
public class SalaryGradeService(
    IRepository<SalaryGrade> salaryGradeRepository,
    IEventPublisher eventPublisher,
    IHttpContextAccessor httpContextAccessor) : ISalaryGradeService
{
    private readonly IRepository<SalaryGrade> _salaryGradeRepository = salaryGradeRepository;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <inheritdoc />
    public async Task<Result<SalaryGradeResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        SalaryGrade? salaryGrade = await _salaryGradeRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        return salaryGrade == null
            ? Result<SalaryGradeResponseDto>.NotFound("Salary grade not found.")
            : Result<SalaryGradeResponseDto>.Success(salaryGrade.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<PagedResult<SalaryGradeResponseDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<SalaryGrade> queryable = _salaryGradeRepository.Query();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            string searchTerm = query.SearchTerm.ToLower();
            queryable = queryable.Where(sg =>
                sg.SalaryGradeName.ToLower().Contains(searchTerm) ||
                (sg.Description != null && sg.Description.ToLower().Contains(searchTerm)));
        }

        int totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(sg => sg.SalaryGradeName)
            : queryable.OrderBy(sg => sg.SalaryGradeName);

        List<SalaryGradeResponseDto> items = await queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(sg => new SalaryGradeResponseDto
            {
                DisplayId = sg.DisplayId,
                SalaryGradeName = sg.SalaryGradeName,
                Description = sg.Description,
                Step = sg.Step,
                MonthlySalary = sg.MonthlySalary,
                IsActive = sg.IsActive,
                CreatedOn = sg.CreatedOn,
                CreatedBy = sg.CreatedBy,
                ModifiedOn = sg.ModifiedOn,
                ModifiedBy = sg.ModifiedBy
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SalaryGradeResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<Result<SalaryGradeResponseDto>> CreateAsync(CreateSalaryGradeDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        SalaryGrade salaryGrade = new()
        {
            SalaryGradeName = dto.SalaryGradeName,
            Description = dto.Description,
            Step = dto.Step,
            MonthlySalary = dto.MonthlySalary,
            CreatedBy = createdBy
        };

        _ = await _salaryGradeRepository.AddAsync(salaryGrade, cancellationToken);

        // Publish domain event
        await PublishSalaryGradeCreatedEventAsync(salaryGrade, createdBy, cancellationToken);

        return Result<SalaryGradeResponseDto>.Success(salaryGrade.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result<SalaryGradeResponseDto>> UpdateAsync(long displayId, UpdateSalaryGradeDto dto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        SalaryGrade? salaryGrade = await _salaryGradeRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (salaryGrade == null)
        {
            return Result<SalaryGradeResponseDto>.NotFound("Salary grade not found.");
        }

        salaryGrade.SalaryGradeName = dto.SalaryGradeName;
        salaryGrade.Description = dto.Description;
        salaryGrade.Step = dto.Step;
        salaryGrade.MonthlySalary = dto.MonthlySalary;
        salaryGrade.IsActive = dto.IsActive;
        salaryGrade.ModifiedBy = modifiedBy;

        await _salaryGradeRepository.UpdateAsync(salaryGrade, cancellationToken);

        // Publish domain event
        Dictionary<string, object?> changes = new()
        {
            ["SalaryGradeName"] = dto.SalaryGradeName,
            ["Description"] = dto.Description,
            ["Step"] = dto.Step,
            ["MonthlySalary"] = dto.MonthlySalary,
            ["IsActive"] = dto.IsActive
        };
        await PublishSalaryGradeUpdatedEventAsync(salaryGrade, changes, modifiedBy, cancellationToken);

        return Result<SalaryGradeResponseDto>.Success(salaryGrade.ToResponseDto());
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default)
    {
        SalaryGrade? salaryGrade = await _salaryGradeRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        if (salaryGrade == null)
        {
            return Result.NotFound("Salary grade not found.");
        }

        salaryGrade.ModifiedBy = deletedBy;
        await _salaryGradeRepository.DeleteAsync(salaryGrade, cancellationToken);

        // Publish domain event
        await PublishSalaryGradeDeletedEventAsync(salaryGrade, deletedBy, cancellationToken);

        return Result.Success();
    }

    #region Event Publishing Helpers

    private async Task PublishSalaryGradeCreatedEventAsync(SalaryGrade salaryGrade, string userId, CancellationToken cancellationToken)
    {
        try
        {
            SalaryGradeCreatedEvent domainEvent = new(
                salaryGradeId: salaryGrade.Id,
                salaryGradeName: salaryGrade.SalaryGradeName,
                step: salaryGrade.Step,
                monthlySalary: salaryGrade.MonthlySalary,
                isActive: salaryGrade.IsActive
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
            Console.WriteLine($"Failed to publish SalaryGradeCreatedEvent: {ex.Message}");
        }
    }

    private async Task PublishSalaryGradeUpdatedEventAsync(
        SalaryGrade salaryGrade,
        Dictionary<string, object?> changes,
        string userId,
        CancellationToken cancellationToken)
    {
        try
        {
            SalaryGradeUpdatedEvent domainEvent = new(salaryGrade.Id, changes);
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
            Console.WriteLine($"Failed to publish SalaryGradeUpdatedEvent: {ex.Message}");
        }
    }

    private async Task PublishSalaryGradeDeletedEventAsync(SalaryGrade salaryGrade, string userId, CancellationToken cancellationToken)
    {
        try
        {
            SalaryGradeDeletedEvent domainEvent = new(salaryGrade.Id, salaryGrade.SalaryGradeName);
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
            Console.WriteLine($"Failed to publish SalaryGradeDeletedEvent: {ex.Message}");
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
                Source = "SalaryGradeService"
            };
    }

    #endregion
}
