using EmployeeManagementSystem.Api.Controllers;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Item;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing items.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ItemsController"/> class.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class ItemsController(IItemService itemService) : ApiControllerBase
{
    private readonly IItemService _itemService = itemService;

    /// <summary>
    /// Gets a paginated list of items.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of items.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ItemResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ItemResponseDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _itemService.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets an item by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The item details.</returns>
    [HttpGet("{displayId:long}")]
    [ProducesResponseType(typeof(ItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponseDto>> GetByDisplayId(
        long displayId,
        CancellationToken cancellationToken)
    {
        var result = await _itemService.GetByDisplayIdAsync(displayId, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Creates a new item.
    /// </summary>
    /// <param name="dto">The item creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created item.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ItemResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemResponseDto>> Create(
        [FromBody] CreateItemDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _itemService.CreateAsync(dto, CurrentUser, cancellationToken);
        return ToCreatedResult(result, nameof(GetByDisplayId), new { displayId = result.Value?.DisplayId });
    }

    /// <summary>
    /// Updates an existing item.
    /// </summary>
    /// <param name="displayId">The display ID of the item to update.</param>
    /// <param name="dto">The item update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated item.</returns>
    [HttpPut("{displayId:long}")]
    [ProducesResponseType(typeof(ItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponseDto>> Update(
        long displayId,
        [FromBody] UpdateItemDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _itemService.UpdateAsync(displayId, dto, CurrentUser, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes an item by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the item to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{displayId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long displayId,
        CancellationToken cancellationToken)
    {
        var result = await _itemService.DeleteAsync(displayId, CurrentUser, cancellationToken);
        return ToNoContentResult(result);
    }
}
