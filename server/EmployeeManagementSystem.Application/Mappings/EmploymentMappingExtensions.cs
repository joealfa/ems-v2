using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Employment entities to DTOs.
/// </summary>
public static class EmploymentMappingExtensions
{
    /// <summary>
    /// Extension method for employment.
    /// </summary>
    /// <param name="employment">The employment entity to map.</param>
    extension(Employment employment)
    {
        /// <summary>
        /// Maps an Employment entity to an EmploymentResponseDto.
        /// </summary>
        /// <returns>The mapped EmploymentResponseDto.</returns>
        public EmploymentResponseDto ToResponseDto()
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
        /// <returns>The mapped EmploymentListDto.</returns>
        public EmploymentListDto ToListDto()
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
    }

    /// <summary>
    /// Extension method for employment school.
    /// </summary>
    /// <param name="employmentSchool">The employment school entity to map.</param>
    extension(EmploymentSchool employmentSchool)
    {
        /// <summary>
        /// Maps an EmploymentSchool entity to an EmploymentSchoolResponseDto.
        /// </summary>
        /// <returns>The mapped EmploymentSchoolResponseDto.</returns>
        public EmploymentSchoolResponseDto ToResponseDto()
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
    }

    /// <summary>
    /// Extension method for employment schools collection.
    /// </summary>
    /// <param name="employmentSchools">The employment school entities to map.</param>
    extension(IEnumerable<EmploymentSchool> employmentSchools)
    {
        /// <summary>
        /// Maps a collection of EmploymentSchool entities to EmploymentSchoolResponseDto list.
        /// </summary>
        /// <returns>The mapped list of EmploymentSchoolResponseDto.</returns>
        public IReadOnlyList<EmploymentSchoolResponseDto> ToResponseDtoList()
        {
            return employmentSchools.Select(es => es.ToResponseDto()).ToList();
        }
    }
}
