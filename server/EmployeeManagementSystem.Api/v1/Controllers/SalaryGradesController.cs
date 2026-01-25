using EmployeeManagementSystem.Api.Controllers;
using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.SalaryGrade;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing salary grades.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SalaryGradesController"/> class.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class SalaryGradesController(ISalaryGradeService salaryGradeService) : ApiControllerBase
{
    private readonly ISalaryGradeService _salaryGradeService = salaryGradeService;

    /// <summary>
    /// Gets a paginated list of salary grades.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of salary grades.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SalaryGradeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SalaryGradeResponseDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _salaryGradeService.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a salary grade by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the salary grade.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The salary grade details.</returns>
    [HttpGet("{displayId:long}")]
    [ProducesResponseType(typeof(SalaryGradeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SalaryGradeResponseDto>> GetByDisplayId(
        long displayId,
        CancellationToken cancellationToken)
    {
        var result = await _salaryGradeService.GetByDisplayIdAsync(displayId, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Creates a new salary grade.
    /// </summary>
    /// <param name="dto">The salary grade creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created salary grade.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SalaryGradeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SalaryGradeResponseDto>> Create(
        [FromBody] CreateSalaryGradeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _salaryGradeService.CreateAsync(dto, CurrentUserEmail, cancellationToken);
        return ToCreatedResult(result, result.Value?.DisplayId ?? 0);
    }

    /// <summary>
    /// Updates an existing salary grade.
    /// </summary>
    /// <param name="displayId">The display ID of the salary grade to update.</param>
    /// <param name="dto">The salary grade update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated salary grade.</returns>
    [HttpPut("{displayId:long}")]
    [ProducesResponseType(typeof(SalaryGradeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SalaryGradeResponseDto>> Update(
        long displayId,
        [FromBody] UpdateSalaryGradeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _salaryGradeService.UpdateAsync(displayId, dto, CurrentUserEmail, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes a salary grade by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the salary grade to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{displayId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long displayId,
        CancellationToken cancellationToken)
    {
        var result = await _salaryGradeService.DeleteAsync(displayId, CurrentUserEmail, cancellationToken);
        return ToNoContentResult(result);
    }
}
