using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.School;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing schools.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class SchoolsController : ControllerBase
{
    private readonly ISchoolService _schoolService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchoolsController"/> class.
    /// </summary>
    public SchoolsController(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    /// <summary>
    /// Gets a paginated list of schools.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of schools.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SchoolListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SchoolListDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _schoolService.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a school by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the school.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The school details.</returns>
    [HttpGet("{displayId:long}")]
    [ProducesResponseType(typeof(SchoolResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SchoolResponseDto>> GetByDisplayId(
        long displayId,
        CancellationToken cancellationToken)
    {
        var result = await _schoolService.GetByDisplayIdAsync(displayId, cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Creates a new school.
    /// </summary>
    /// <param name="dto">The school creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created school.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SchoolResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SchoolResponseDto>> Create(
        [FromBody] CreateSchoolDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // TODO: Get actual user from authentication
        var createdBy = "System";

        var result = await _schoolService.CreateAsync(dto, createdBy, cancellationToken);
        return CreatedAtAction(nameof(GetByDisplayId), new { displayId = result.DisplayId }, result);
    }

    /// <summary>
    /// Updates an existing school.
    /// </summary>
    /// <param name="displayId">The display ID of the school to update.</param>
    /// <param name="dto">The school update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated school.</returns>
    [HttpPut("{displayId:long}")]
    [ProducesResponseType(typeof(SchoolResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SchoolResponseDto>> Update(
        long displayId,
        [FromBody] UpdateSchoolDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // TODO: Get actual user from authentication
        var modifiedBy = "System";

        var result = await _schoolService.UpdateAsync(displayId, dto, modifiedBy, cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Deletes a school by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the school to delete.</param>
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

        var result = await _schoolService.DeleteAsync(displayId, deletedBy, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
