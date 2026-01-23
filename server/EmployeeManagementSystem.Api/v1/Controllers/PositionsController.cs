using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Position;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing positions.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PositionsController : ControllerBase
{
    private readonly IPositionService _positionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionsController"/> class.
    /// </summary>
    public PositionsController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    /// <summary>
    /// Gets a paginated list of positions.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of positions.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PositionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PositionResponseDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _positionService.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a position by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the position.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The position details.</returns>
    [HttpGet("{displayId:long}")]
    [ProducesResponseType(typeof(PositionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PositionResponseDto>> GetByDisplayId(
        long displayId,
        CancellationToken cancellationToken)
    {
        var result = await _positionService.GetByDisplayIdAsync(displayId, cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Creates a new position.
    /// </summary>
    /// <param name="dto">The position creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created position.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PositionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PositionResponseDto>> Create(
        [FromBody] CreatePositionDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // TODO: Get actual user from authentication
        var createdBy = "System";

        var result = await _positionService.CreateAsync(dto, createdBy, cancellationToken);
        return CreatedAtAction(nameof(GetByDisplayId), new { displayId = result.DisplayId }, result);
    }

    /// <summary>
    /// Updates an existing position.
    /// </summary>
    /// <param name="displayId">The display ID of the position to update.</param>
    /// <param name="dto">The position update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated position.</returns>
    [HttpPut("{displayId:long}")]
    [ProducesResponseType(typeof(PositionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PositionResponseDto>> Update(
        long displayId,
        [FromBody] UpdatePositionDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // TODO: Get actual user from authentication
        var modifiedBy = "System";

        var result = await _positionService.UpdateAsync(displayId, dto, modifiedBy, cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Deletes a position by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the position to delete.</param>
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

        var result = await _positionService.DeleteAsync(displayId, deletedBy, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
