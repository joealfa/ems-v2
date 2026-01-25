using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.SalaryGrade;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for salary grade operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SalaryGradeService"/> class.
/// </remarks>
public class SalaryGradeService(IRepository<SalaryGrade> salaryGradeRepository) : ISalaryGradeService
{
    private readonly IRepository<SalaryGrade> _salaryGradeRepository = salaryGradeRepository;

    /// <inheritdoc />
    public async Task<Result<SalaryGradeResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        SalaryGrade? salaryGrade = await _salaryGradeRepository.GetByDisplayIdAsync(displayId, cancellationToken);
        return salaryGrade == null ? Result<SalaryGradeResponseDto>.NotFound("Salary grade not found.") : Result<SalaryGradeResponseDto>.Success(MapToResponseDto(salaryGrade));
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

        return Result<SalaryGradeResponseDto>.Success(MapToResponseDto(salaryGrade));
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

        return Result<SalaryGradeResponseDto>.Success(MapToResponseDto(salaryGrade));
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
        return Result.Success();
    }

    private static SalaryGradeResponseDto MapToResponseDto(SalaryGrade salaryGrade)
    {
        return new SalaryGradeResponseDto
        {
            DisplayId = salaryGrade.DisplayId,
            SalaryGradeName = salaryGrade.SalaryGradeName,
            Description = salaryGrade.Description,
            Step = salaryGrade.Step,
            MonthlySalary = salaryGrade.MonthlySalary,
            IsActive = salaryGrade.IsActive,
            CreatedOn = salaryGrade.CreatedOn,
            CreatedBy = salaryGrade.CreatedBy,
            ModifiedOn = salaryGrade.ModifiedOn,
            ModifiedBy = salaryGrade.ModifiedBy
        };
    }
}
