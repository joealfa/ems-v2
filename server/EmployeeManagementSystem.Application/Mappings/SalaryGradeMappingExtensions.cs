using EmployeeManagementSystem.Application.DTOs.SalaryGrade;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping SalaryGrade entities to DTOs.
/// </summary>
public static class SalaryGradeMappingExtensions
{
    /// <summary>
    /// Maps a SalaryGrade entity to a SalaryGradeResponseDto.
    /// </summary>
    /// <param name="salaryGrade">The salary grade entity to map.</param>
    /// <returns>The mapped SalaryGradeResponseDto.</returns>
    public static SalaryGradeResponseDto ToResponseDto(this SalaryGrade salaryGrade)
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
