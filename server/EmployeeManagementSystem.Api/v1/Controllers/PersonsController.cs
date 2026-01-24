using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Person;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing persons.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class PersonsController : ApiControllerBase
{
    private readonly IPersonService _personService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonsController"/> class.
    /// </summary>
    public PersonsController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Gets a paginated list of persons.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of persons.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PersonListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PersonListDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _personService.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a person by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the person.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The person details.</returns>
    [HttpGet("{displayId:long}")]
    [ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonResponseDto>> GetByDisplayId(
        long displayId,
        CancellationToken cancellationToken)
    {
        var result = await _personService.GetByDisplayIdAsync(displayId, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Creates a new person.
    /// </summary>
    /// <param name="dto">The person creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created person.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonResponseDto>> Create(
        [FromBody] CreatePersonDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _personService.CreateAsync(dto, CurrentUserEmail, cancellationToken);
        return ToCreatedResult(result, result.Value?.DisplayId ?? 0);
    }

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="displayId">The display ID of the person to update.</param>
    /// <param name="dto">The person update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated person.</returns>
    [HttpPut("{displayId:long}")]
    [ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonResponseDto>> Update(
        long displayId,
        [FromBody] UpdatePersonDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _personService.UpdateAsync(displayId, dto, CurrentUserEmail, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes a person by display ID.
    /// </summary>
    /// <param name="displayId">The display ID of the person to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{displayId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long displayId,
        CancellationToken cancellationToken)
    {
        var result = await _personService.DeleteAsync(displayId, CurrentUserEmail, cancellationToken);
        return ToNoContentResult(result);
    }
}
