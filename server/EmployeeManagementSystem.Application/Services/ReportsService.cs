using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service for generating reports and statistics.
/// </summary>
public class ReportsService(
    IRepository<Person> personRepository,
    IRepository<Employment> employmentRepository,
    IRepository<School> schoolRepository,
    IRepository<Position> positionRepository,
    IRepository<SalaryGrade> salaryGradeRepository,
    IRepository<Item> itemRepository) : IReportsService
{
    private readonly IRepository<Person> _personRepository = personRepository;
    private readonly IRepository<Employment> _employmentRepository = employmentRepository;
    private readonly IRepository<School> _schoolRepository = schoolRepository;
    private readonly IRepository<Position> _positionRepository = positionRepository;
    private readonly IRepository<SalaryGrade> _salaryGradeRepository = salaryGradeRepository;
    private readonly IRepository<Item> _itemRepository = itemRepository;

    /// <inheritdoc />
    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        // Count total persons (not deleted)
        var totalPersons = await _personRepository.Query()
            .CountAsync(cancellationToken);

        // Count active employments (IsActive = true and not deleted)
        var activeEmployments = await _employmentRepository.Query()
            .Where(e => e.IsActive)
            .CountAsync(cancellationToken);

        // Count total schools (not deleted)
        var totalSchools = await _schoolRepository.Query()
            .CountAsync(cancellationToken);

        // Count total positions (not deleted)
        var totalPositions = await _positionRepository.Query()
            .CountAsync(cancellationToken);

        // Count total salary grades (not deleted)
        var totalSalaryGrades = await _salaryGradeRepository.Query()
            .CountAsync(cancellationToken);

        // Count total items (not deleted)
        var totalItems = await _itemRepository.Query()
            .CountAsync(cancellationToken);

        return new DashboardStatsDto
        {
            TotalPersons = totalPersons,
            ActiveEmployments = activeEmployments,
            TotalSchools = totalSchools,
            TotalPositions = totalPositions,
            TotalSalaryGrades = totalSalaryGrades,
            TotalItems = totalItems
        };
    }
}
