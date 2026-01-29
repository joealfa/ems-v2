using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Employment entities to DTOs.
/// </summary>
public static class EmploymentMappingExtensions
{
    /// <summary>
    /// Maps an Employment entity to an EmploymentResponseDto.
    /// </summary>
    /// <param name="employment">The employment entity to map.</param>
    /// <returns>The mapped EmploymentResponseDto.</returns>
    public static EmploymentResponseDto ToResponseDto(this Employment employment)
    {
        return new EmploymentResponseDto
        {
            DisplayId = employment.DisplayId,
            DepEdId = employment.DepEdId,
            PSIPOPItemNumber = employment.PSIPOPItemNumber,
            TINId = employment.TINId,
            GSISId = employment.GSISId,
            PhilHealthId = employment.PhilHealthId,
            DateOfOriginalAppointment = employment.DateOfOriginalAppointment,
            AppointmentStatus = employment.AppointmentStatus,
            EmploymentStatus = employment.EmploymentStatus,
            Eligibility = employment.Eligibility,
            IsActive = employment.IsActive,
            CreatedOn = employment.CreatedOn,
            CreatedBy = employment.CreatedBy,
            ModifiedOn = employment.ModifiedOn,
            ModifiedBy = employment.ModifiedBy,
            Person = employment.Person.ToEmploymentPersonDto(),
            Position = employment.Position.ToEmploymentPositionDto(),
            SalaryGrade = employment.SalaryGrade.ToEmploymentSalaryGradeDto(),
            Item = employment.Item.ToEmploymentItemDto(),
            Schools = employment.EmploymentSchools.ToResponseDtoList()
        };
    }

    /// <summary>
    /// Maps an Employment entity to an EmploymentListDto.
    /// </summary>
    /// <param name="employment">The employment entity to map.</param>
    /// <returns>The mapped EmploymentListDto.</returns>
    public static EmploymentListDto ToListDto(this Employment employment)
    {
        return new EmploymentListDto
        {
            DisplayId = employment.DisplayId,
            DepEdId = employment.DepEdId,
            EmployeeFullName = employment.Person.FullName,
            PositionTitle = employment.Position.TitleName,
            EmploymentStatus = employment.EmploymentStatus,
            IsActive = employment.IsActive,
            CreatedOn = employment.CreatedOn,
            CreatedBy = employment.CreatedBy,
            ModifiedOn = employment.ModifiedOn,
            ModifiedBy = employment.ModifiedBy
        };
    }

    /// <summary>
    /// Maps a Person entity to an EmploymentPersonDto (simplified view for employment context).
    /// </summary>
    /// <param name="person">The person entity to map.</param>
    /// <returns>The mapped EmploymentPersonDto.</returns>
    public static EmploymentPersonDto ToEmploymentPersonDto(this Person person)
    {
        return new EmploymentPersonDto
        {
            DisplayId = person.DisplayId,
            FullName = person.FullName
        };
    }

    /// <summary>
    /// Maps a Position entity to an EmploymentPositionDto (simplified view for employment context).
    /// </summary>
    /// <param name="position">The position entity to map.</param>
    /// <returns>The mapped EmploymentPositionDto.</returns>
    public static EmploymentPositionDto ToEmploymentPositionDto(this Position position)
    {
        return new EmploymentPositionDto
        {
            DisplayId = position.DisplayId,
            TitleName = position.TitleName
        };
    }

    /// <summary>
    /// Maps a SalaryGrade entity to an EmploymentSalaryGradeDto (simplified view for employment context).
    /// </summary>
    /// <param name="salaryGrade">The salary grade entity to map.</param>
    /// <returns>The mapped EmploymentSalaryGradeDto.</returns>
    public static EmploymentSalaryGradeDto ToEmploymentSalaryGradeDto(this SalaryGrade salaryGrade)
    {
        return new EmploymentSalaryGradeDto
        {
            DisplayId = salaryGrade.DisplayId,
            SalaryGradeName = salaryGrade.SalaryGradeName,
            Step = salaryGrade.Step,
            MonthlySalary = salaryGrade.MonthlySalary
        };
    }

    /// <summary>
    /// Maps an Item entity to an EmploymentItemDto (simplified view for employment context).
    /// </summary>
    /// <param name="item">The item entity to map.</param>
    /// <returns>The mapped EmploymentItemDto.</returns>
    public static EmploymentItemDto ToEmploymentItemDto(this Item item)
    {
        return new EmploymentItemDto
        {
            DisplayId = item.DisplayId,
            ItemName = item.ItemName
        };
    }

    /// <summary>
    /// Maps an EmploymentSchool entity to an EmploymentSchoolResponseDto.
    /// </summary>
    /// <param name="employmentSchool">The employment school entity to map.</param>
    /// <returns>The mapped EmploymentSchoolResponseDto.</returns>
    public static EmploymentSchoolResponseDto ToResponseDto(this EmploymentSchool employmentSchool)
    {
        return new EmploymentSchoolResponseDto
        {
            DisplayId = employmentSchool.DisplayId,
            SchoolDisplayId = employmentSchool.School.DisplayId,
            SchoolName = employmentSchool.School.SchoolName,
            StartDate = employmentSchool.StartDate,
            EndDate = employmentSchool.EndDate,
            IsCurrent = employmentSchool.IsCurrent,
            IsActive = employmentSchool.IsActive,
            CreatedOn = employmentSchool.CreatedOn,
            CreatedBy = employmentSchool.CreatedBy,
            ModifiedOn = employmentSchool.ModifiedOn,
            ModifiedBy = employmentSchool.ModifiedBy
        };
    }

    /// <summary>
    /// Maps a collection of EmploymentSchool entities to EmploymentSchoolResponseDto list.
    /// </summary>
    /// <param name="employmentSchools">The employment school entities to map.</param>
    /// <returns>The mapped list of EmploymentSchoolResponseDto.</returns>
    public static IReadOnlyList<EmploymentSchoolResponseDto> ToResponseDtoList(this IEnumerable<EmploymentSchool> employmentSchools)
    {
        return employmentSchools.Select(es => es.ToResponseDto()).ToList();
    }
}
