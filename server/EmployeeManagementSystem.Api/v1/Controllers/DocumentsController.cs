using EmployeeManagementSystem.Api.Controllers;
using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Document;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing person documents.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DocumentsController"/> class.
/// </remarks>
[ApiController]
[Route("api/v1/Persons/{displayId:long}/[controller]")]
[Produces("application/json")]
[Authorize]
public class DocumentsController(IDocumentService documentService) : ApiControllerBase
{
    private readonly IDocumentService _documentService = documentService;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "image/jpeg",
        "image/png"
    };

    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png"
    };

    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB
    private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5 MB

    /// <summary>
    /// Gets a paginated list of documents for a person.
    /// </summary>
    /// <param name="displayId">The person's display ID.</param>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of documents.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DocumentListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DocumentListDto>>> GetAll(
        long displayId,
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        PagedResult<DocumentListDto> result = await _documentService.GetPagedAsync(displayId, query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a document by display ID.
    /// </summary>
    /// <param name="displayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document details.</returns>
    [HttpGet("{documentDisplayId:long}")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentResponseDto>> GetByDisplayId(
        long displayId,
        long documentDisplayId,
        CancellationToken cancellationToken)
    {
        Result<DocumentResponseDto> result = await _documentService.GetByDisplayIdAsync(displayId, documentDisplayId, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Uploads a document for a person.
    /// </summary>
    /// <param name="displayId">The person's display ID.</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="description">Optional description for the document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created document details.</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<ActionResult<DocumentResponseDto>> Upload(
        long displayId,
        IFormFile file,
        [FromForm] string? description,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return BadRequest($"File size exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.");
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            return BadRequest($"File type '{file.ContentType}' is not allowed.");
        }

        UploadDocumentDto dto = new()
        {
            FileStream = file.OpenReadStream(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            Description = description
        };

        Result<DocumentResponseDto> result = await _documentService.UploadAsync(displayId, dto, CurrentUserEmail, cancellationToken);
        return ToCreatedResult(result, nameof(GetByDisplayId), new { displayId, documentDisplayId = result.Value?.DisplayId ?? 0 });
    }

    /// <summary>
    /// Updates document metadata.
    /// </summary>
    /// <param name="displayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="dto">The update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated document details.</returns>
    [HttpPut("{documentDisplayId:long}")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentResponseDto>> Update(
        long displayId,
        long documentDisplayId,
        [FromBody] UpdateDocumentDto dto,
        CancellationToken cancellationToken)
    {
        Result<DocumentResponseDto> result = await _documentService.UpdateAsync(displayId, documentDisplayId, dto, CurrentUserEmail, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Downloads a document.
    /// </summary>
    /// <param name="displayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document file.</returns>
    [HttpGet("{documentDisplayId:long}/download")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(
        long displayId,
        long documentDisplayId,
        CancellationToken cancellationToken)
    {
        Result<BlobDownloadResultDto> result = await _documentService.DownloadAsync(displayId, documentDisplayId, cancellationToken);
        return ToFileResult(result);
    }

    /// <summary>
    /// Deletes a document.
    /// </summary>
    /// <param name="displayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{documentDisplayId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long displayId,
        long documentDisplayId,
        CancellationToken cancellationToken)
    {
        Result result = await _documentService.DeleteAsync(displayId, documentDisplayId, CurrentUserEmail, cancellationToken);
        return ToNoContentResult(result);
    }

    /// <summary>
    /// Uploads a profile image for a person.
    /// </summary>
    /// <param name="displayId">The person's display ID.</param>
    /// <param name="file">The image file to upload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded profile image.</returns>
    [HttpPost("profile-image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequestSizeLimit(MaxImageSizeBytes)]
    public async Task<ActionResult<string>> UploadProfileImage(
        long displayId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        if (file.Length > MaxImageSizeBytes)
        {
            return BadRequest($"Image size exceeds the maximum allowed size of {MaxImageSizeBytes / (1024 * 1024)} MB.");
        }

        if (!AllowedImageContentTypes.Contains(file.ContentType))
        {
            return BadRequest($"File type '{file.ContentType}' is not allowed for profile images. Only JPEG and PNG are allowed.");
        }

        UploadDocumentDto dto = new()
        {
            FileStream = file.OpenReadStream(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length
        };

        Result<string> result = await _documentService.UploadProfileImageAsync(displayId, dto, CurrentUserEmail, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Gets a person's profile image.
    /// </summary>
    /// <param name="displayId">The person's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The profile image file.</returns>
    [AllowAnonymous]
    [HttpGet("profile-image")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileImage(
        long displayId,
        CancellationToken cancellationToken)
    {
        Result<BlobDownloadResultDto> result = await _documentService.GetProfileImageAsync(displayId, cancellationToken);
        return ToFileResult(result);
    }

    /// <summary>
    /// Deletes a person's profile image.
    /// </summary>
    /// <param name="displayId">The person's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("profile-image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfileImage(
        long displayId,
        CancellationToken cancellationToken)
    {
        Result result = await _documentService.DeleteProfileImageAsync(displayId, CurrentUserEmail, cancellationToken);
        return ToNoContentResult(result);
    }
}
