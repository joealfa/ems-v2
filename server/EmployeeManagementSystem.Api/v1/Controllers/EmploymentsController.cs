using EmployeeManagementSystem.Api.Controllers;
using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing employments.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmploymentsController"/> class.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class EmploymentsController(IEmploymentService employmentService) : ApiControllerBase
{
    private readonly IEmploymentService _employmentService = employmentService;

    /// <summary>
    /// Gets a paginated list of employments.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of employments.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmploymentListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EmploymentListDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        PagedResult<EmploymentListDto> result = await _employmentService.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets an employment by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the employment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The employment details.</returns>
    [HttpGet("{displayId:long}")]
    [ProducesResponseType(typeof(EmploymentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmploymentResponseDto>> GetByDisplayId(
        long displayId,
        CancellationToken cancellationToken)
    {
        Result<EmploymentResponseDto> result = await _employmentService.GetByDisplayIdAsync(displayId, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Creates a new employment.
    /// </summary>
    /// <param name="dto">The employment creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created employment.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(EmploymentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmploymentResponseDto>> Create(
        [FromBody] CreateEmploymentDto dto,
        CancellationToken cancellationToken)
    {
        Result<EmploymentResponseDto> result = await _employmentService.CreateAsync(dto, CurrentUserEmail, cancellationToken);
        return ToCreatedResult(result, result.Value?.DisplayId ?? 0);
    }

    /// <summary>
    /// Updates an existing employment.
    /// </summary>
    /// <param name="displayId">The display ID of the employment to update.</param>
    /// <param name="dto">The employment update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated employment.</returns>
    [HttpPut("{displayId:long}")]
    [ProducesResponseType(typeof(EmploymentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmploymentResponseDto>> Update(
        long displayId,
        [FromBody] UpdateEmploymentDto dto,
        CancellationToken cancellationToken)
    {
        Result<EmploymentResponseDto> result = await _employmentService.UpdateAsync(displayId, dto, CurrentUserEmail, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes an employment by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the employment to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{displayId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long displayId,
        CancellationToken cancellationToken)
    {
        Result result = await _employmentService.DeleteAsync(displayId, CurrentUserEmail, cancellationToken);
        return ToNoContentResult(result);
    }

    /// <summary>
    /// Adds a school assignment to an employment.
    /// </summary>
    /// <param name="displayId">The display ID of the employment.</param>
    /// <param name="dto">The school assignment data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created school assignment.</returns>
    [HttpPost("{displayId:long}/schools")]
    [ProducesResponseType(typeof(EmploymentSchoolResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmploymentSchoolResponseDto>> AddSchoolAssignment(
        long displayId,
        [FromBody] CreateEmploymentSchoolDto dto,
        CancellationToken cancellationToken)
    {
        Result<EmploymentSchoolResponseDto> result = await _employmentService.AddSchoolAssignmentAsync(displayId, dto, CurrentUserEmail, cancellationToken);
        return ToCreatedResult(result, result.Value?.DisplayId ?? 0);
    }

    /// <summary>
    /// Removes a school assignment from an employment.
    /// </summary>
    /// <param name="displayId">The display ID of the employment.</param>
    /// <param name="schoolAssignmentDisplayId">The display ID of the school assignment to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{displayId:long}/schools/{schoolAssignmentDisplayId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSchoolAssignment(
        long displayId,
        long schoolAssignmentDisplayId,
        CancellationToken cancellationToken)
    {
        Result result = await _employmentService.RemoveSchoolAssignmentAsync(schoolAssignmentDisplayId, CurrentUserEmail, cancellationToken);
        return ToNoContentResult(result);
    }
}
