using System.Security.Claims;
using EmployeeManagementSystem.Api.Models;
using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs.Document;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers;

/// <summary>
/// Base controller providing common functionality for all API controllers.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Gets the current user's ID from the JWT token.
    /// </summary>
    protected Guid? CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// Gets the current user's email from the JWT token.
    /// </summary>
    protected string CurrentUserEmail => 
        User.FindFirst(ClaimTypes.Email)?.Value 
        ?? User.FindFirst("email")?.Value 
        ?? "System";

    /// <summary>
    /// Gets the current user identifier for audit fields.
    /// Returns the user's email or "System" if not authenticated.
    /// </summary>
    protected string CurrentUser => CurrentUserEmail;

    /// <summary>
    /// Converts a Result&lt;T&gt; to an ActionResult&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>The appropriate ActionResult based on the result state.</returns>
    protected ActionResult<T> ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return ToErrorResult<T>(result.FailureType, result.Error);
    }

    /// <summary>
    /// Converts a Result&lt;T&gt; to a CreatedAtAction result on success.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="actionName">The action name for the location header.</param>
    /// <param name="routeValues">The route values for the location header.</param>
    /// <returns>The appropriate ActionResult based on the result state.</returns>
    protected ActionResult<T> ToCreatedResult<T>(Result<T> result, string actionName, object routeValues)
    {
        if (result.IsSuccess)
        {
            return CreatedAtAction(actionName, routeValues, result.Value);
        }

        return ToErrorResult<T>(result.FailureType, result.Error);
    }

    /// <summary>
    /// Converts a Result&lt;T&gt; to a CreatedAtAction result on success using GetByDisplayId action.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="displayId">The display ID for the location header.</param>
    /// <returns>The appropriate ActionResult based on the result state.</returns>
    protected ActionResult<T> ToCreatedResult<T>(Result<T> result, long displayId)
    {
        if (result.IsSuccess)
        {
            return CreatedAtAction("GetByDisplayId", new { displayId }, result.Value);
        }

        return ToErrorResult<T>(result.FailureType, result.Error);
    }

    /// <summary>
    /// Converts a Result (without value) to an IActionResult.
    /// Returns NoContent on success.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The appropriate IActionResult based on the result state.</returns>
    protected IActionResult ToNoContentResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return ToErrorResult(result.FailureType, result.Error);
    }

    /// <summary>
    /// Converts a Result&lt;T&gt; to a File result on success.
    /// </summary>
    /// <param name="result">The result containing file data.</param>
    /// <param name="contentType">The content type of the file.</param>
    /// <param name="fileName">The file name for download.</param>
    /// <returns>The appropriate IActionResult based on the result state.</returns>
    protected IActionResult ToFileResult(Result<Stream> result, string contentType, string? fileName = null)
    {
        if (result.IsSuccess && result.Value != null)
        {
            return fileName != null 
                ? File(result.Value, contentType, fileName) 
                : File(result.Value, contentType);
        }

        return ToErrorResult(result.FailureType, result.Error);
    }

    /// <summary>
    /// Converts a Result&lt;BlobDownloadResultDto&gt; to a File result on success.
    /// </summary>
    /// <param name="result">The result containing blob download data.</param>
    /// <returns>The appropriate IActionResult based on the result state.</returns>
    protected IActionResult ToFileResult(Result<BlobDownloadResultDto> result)
    {
        if (result.IsSuccess && result.Value != null)
        {
            return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
        }

        return ToErrorResult(result.FailureType, result.Error);
    }

    /// <summary>
    /// Creates an error ActionResult based on the failure type.
    /// </summary>
    private ActionResult<T> ToErrorResult<T>(FailureType failureType, string? error)
    {
        return ToErrorResult(failureType, error) as ActionResult<T> ?? BadRequest(ApiErrorResponse.BadRequest(error));
    }

    /// <summary>
    /// Creates an error IActionResult based on the failure type.
    /// </summary>
    private ObjectResult ToErrorResult(FailureType failureType, string? error)
    {
        return failureType switch
        {
            FailureType.NotFound => NotFound(ApiErrorResponse.NotFound(error)),
            FailureType.BadRequest => BadRequest(ApiErrorResponse.BadRequest(error)),
            FailureType.Unauthorized => Unauthorized(ApiErrorResponse.Unauthorized(error)),
            FailureType.Conflict => Conflict(ApiErrorResponse.Conflict(error)),
            FailureType.InternalError => StatusCode(500, ApiErrorResponse.InternalError(error)),
            _ => BadRequest(ApiErrorResponse.BadRequest(error))
        };
    }
}
