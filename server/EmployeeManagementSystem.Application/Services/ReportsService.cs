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
    IRepository<Item> itemRepository,
    IRecentActivityRepository activityRepository) : IReportsService
{
    private readonly IRepository<Person> _personRepository = personRepository;
    private readonly IRepository<Employment> _employmentRepository = employmentRepository;
    private readonly IRepository<School> _schoolRepository = schoolRepository;
    private readonly IRepository<Position> _positionRepository = positionRepository;
    private readonly IRepository<SalaryGrade> _salaryGradeRepository = salaryGradeRepository;
    private readonly IRepository<Item> _itemRepository = itemRepository;
    private readonly IRecentActivityRepository _activityRepository = activityRepository;

    /// <inheritdoc />
    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        // Count total persons (not deleted)
        int totalPersons = await _personRepository.Query()
            .CountAsync(cancellationToken);

        // Count active employments (IsActive = true and not deleted)
        int activeEmployments = await _employmentRepository.Query()
            .Where(e => e.IsActive)
            .CountAsync(cancellationToken);

        // Count total schools (not deleted)
        int totalSchools = await _schoolRepository.Query()
            .CountAsync(cancellationToken);

        // Count total positions (not deleted)
        int totalPositions = await _positionRepository.Query()
            .CountAsync(cancellationToken);

        // Count total salary grades (not deleted)
        int totalSalaryGrades = await _salaryGradeRepository.Query()
            .CountAsync(cancellationToken);

        // Count total items (not deleted)
        int totalItems = await _itemRepository.Query()
            .CountAsync(cancellationToken);

        // Get birthday celebrants for the current month
        int currentMonth = DateTime.UtcNow.Month;
        List<BirthdayCelebrantDto> birthdayCelebrants = await _personRepository.Query()
            .Where(p => p.DateOfBirth.Month == currentMonth)
            .OrderBy(p => p.DateOfBirth.Day)
            .Select(p => new BirthdayCelebrantDto
            {
                DisplayId = p.DisplayId,
                FirstName = p.FirstName,
                LastName = p.LastName,
                MiddleName = p.MiddleName,
                FullName = p.FirstName + " " + (p.MiddleName != null ? p.MiddleName + " " : "") + p.LastName,
                DateOfBirth = p.DateOfBirth,
                ProfileImageUrl = p.ProfileImageUrl,
                HasProfileImage = p.HasProfileImage
            })
            .ToListAsync(cancellationToken);

        // Get recent activities (last 10)
        List<RecentActivity> recentActivities = await _activityRepository.GetLatestAsync(10, cancellationToken);

        List<RecentActivityDto> recentActivityDtos = recentActivities
            .Select(a => new RecentActivityDto
            {
                Id = a.Id,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Operation = a.Operation,
                Message = a.Message,
                Timestamp = a.Timestamp,
                UserId = a.UserId
            })
            .ToList();

        return new DashboardStatsDto
        {
            TotalPersons = totalPersons,
            ActiveEmployments = activeEmployments,
            TotalSchools = totalSchools,
            TotalPositions = totalPositions,
            TotalSalaryGrades = totalSalaryGrades,
            TotalItems = totalItems,
            BirthdayCelebrants = birthdayCelebrants,
            RecentActivities = recentActivityDtos
        };
    }
}
