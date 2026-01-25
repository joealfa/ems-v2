using EmployeeManagementSystem.Api.Controllers;
using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Position;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing positions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PositionsController"/> class.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class PositionsController(IPositionService positionService) : ApiControllerBase
{
    private readonly IPositionService _positionService = positionService;

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
        PagedResult<PositionResponseDto> result = await _positionService.GetPagedAsync(query, cancellationToken);
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
        Result<PositionResponseDto> result = await _positionService.GetByDisplayIdAsync(displayId, cancellationToken);
        return ToActionResult(result);
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
        Result<PositionResponseDto> result = await _positionService.CreateAsync(dto, CurrentUserEmail, cancellationToken);
        return ToCreatedResult(result, result.Value?.DisplayId ?? 0);
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
        Result<PositionResponseDto> result = await _positionService.UpdateAsync(displayId, dto, CurrentUserEmail, cancellationToken);
        return ToActionResult(result);
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
        Result result = await _positionService.DeleteAsync(displayId, CurrentUserEmail, cancellationToken);
        return ToNoContentResult(result);
    }
}
