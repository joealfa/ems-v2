using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Application.DTOs.SalaryGrade;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping SalaryGrade entities to DTOs.
/// </summary>
public static class SalaryGradeMappingExtensions
{
    /// <summary>
    /// Extension method for salary grade.
    /// </summary>
    /// <param name="salaryGrade">The salary grade entity to map.</param>
    extension(SalaryGrade salaryGrade)
    {
        /// <summary>
        /// Maps a SalaryGrade entity to a SalaryGradeResponseDto.
        /// </summary>
        /// <returns>The mapped SalaryGradeResponseDto.</returns>
        public SalaryGradeResponseDto ToResponseDto()
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

        /// <summary>
        /// Maps a SalaryGrade entity to an EmploymentSalaryGradeDto (simplified view for employment context).
        /// </summary>
        /// <returns>The mapped EmploymentSalaryGradeDto.</returns>
        public EmploymentSalaryGradeDto ToEmploymentSalaryGradeDto()
        {
            return new EmploymentSalaryGradeDto
            {
                DisplayId = salaryGrade.DisplayId,
                SalaryGradeName = salaryGrade.SalaryGradeName,
                Step = salaryGrade.Step,
                MonthlySalary = salaryGrade.MonthlySalary
            };
        }
    }
}
