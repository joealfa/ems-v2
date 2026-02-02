using EmployeeManagementSystem.Api.Controllers;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for reports and statistics.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ReportsController"/> class.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class ReportsController(IReportsService reportsService) : ApiControllerBase
{
    private readonly IReportsService _reportsService = reportsService;

    /// <summary>
    /// Gets dashboard statistics including counts of persons, employments, schools, and positions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dashboard statistics.</returns>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats(CancellationToken cancellationToken)
    {
        DashboardStatsDto stats = await _reportsService.GetDashboardStatsAsync(cancellationToken);
        return Ok(stats);
    }
}
