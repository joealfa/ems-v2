using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing employments.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class EmploymentsController : ControllerBase
{
    private readonly IEmploymentService _employmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmploymentsController"/> class.
    /// </summary>
    public EmploymentsController(IEmploymentService employmentService)
    {
        _employmentService = employmentService;
    }

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
        var result = await _employmentService.GetPagedAsync(query, cancellationToken);
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
        var result = await _employmentService.GetByDisplayIdAsync(displayId, cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // TODO: Get actual user from authentication
            var createdBy = "System";

            var result = await _employmentService.CreateAsync(dto, createdBy, cancellationToken);
            return CreatedAtAction(nameof(GetByDisplayId), new { displayId = result.DisplayId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // TODO: Get actual user from authentication
            var modifiedBy = "System";

            var result = await _employmentService.UpdateAsync(displayId, dto, modifiedBy, cancellationToken);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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
        // TODO: Get actual user from authentication
        var deletedBy = "System";

        var result = await _employmentService.DeleteAsync(displayId, deletedBy, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // TODO: Get actual user from authentication
            var createdBy = "System";

            var result = await _employmentService.AddSchoolAssignmentAsync(displayId, dto, createdBy, cancellationToken);
            if (result == null)
                return NotFound();

            return Created($"api/v1/employments/{displayId}/schools/{result.DisplayId}", result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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
        // TODO: Get actual user from authentication
        var deletedBy = "System";

        var result = await _employmentService.RemoveSchoolAssignmentAsync(schoolAssignmentDisplayId, deletedBy, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
