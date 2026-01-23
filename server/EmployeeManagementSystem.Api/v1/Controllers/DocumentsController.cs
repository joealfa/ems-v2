using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Document;
using EmployeeManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.v1.Controllers;

/// <summary>
/// API controller for managing person documents.
/// </summary>
[ApiController]
[Route("api/v1/persons/{personDisplayId:long}/documents")]
[Produces("application/json")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

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
    /// Initializes a new instance of the <see cref="DocumentsController"/> class.
    /// </summary>
    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    /// <summary>
    /// Gets a paginated list of documents for a person.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of documents.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DocumentListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DocumentListDto>>> GetAll(
        long personDisplayId,
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _documentService.GetPagedAsync(personDisplayId, query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a document by display ID.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document details.</returns>
    [HttpGet("{documentDisplayId:long}")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentResponseDto>> GetByDisplayId(
        long personDisplayId,
        long documentDisplayId,
        CancellationToken cancellationToken)
    {
        var result = await _documentService.GetByDisplayIdAsync(personDisplayId, documentDisplayId, cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Uploads a document for a person.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
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
        long personDisplayId,
        IFormFile file,
        [FromForm] string? description,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        if (file.Length > MaxFileSizeBytes)
            return BadRequest($"File size exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.");

        if (!AllowedContentTypes.Contains(file.ContentType))
            return BadRequest($"File type '{file.ContentType}' is not allowed.");

        // TODO: Get actual user from authentication
        var createdBy = "System";

        try
        {
            var dto = new UploadDocumentDto
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                Description = description
            };

            var result = await _documentService.UploadAsync(personDisplayId, dto, createdBy, cancellationToken);
            if (result == null)
                return NotFound("Person not found.");

            return CreatedAtAction(
                nameof(GetByDisplayId),
                new { personDisplayId, documentDisplayId = result.DisplayId },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates document metadata.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="dto">The update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated document details.</returns>
    [HttpPut("{documentDisplayId:long}")]
    [ProducesResponseType(typeof(DocumentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentResponseDto>> Update(
        long personDisplayId,
        long documentDisplayId,
        [FromBody] UpdateDocumentDto dto,
        CancellationToken cancellationToken)
    {
        // TODO: Get actual user from authentication
        var modifiedBy = "System";

        var result = await _documentService.UpdateAsync(personDisplayId, documentDisplayId, dto, modifiedBy, cancellationToken);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Downloads a document.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document file.</returns>
    [HttpGet("{documentDisplayId:long}/download")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(
        long personDisplayId,
        long documentDisplayId,
        CancellationToken cancellationToken)
    {
        var result = await _documentService.DownloadAsync(personDisplayId, documentDisplayId, cancellationToken);
        if (result == null)
            return NotFound();

        return File(result.Content, result.ContentType, result.FileName);
    }

    /// <summary>
    /// Deletes a document.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{documentDisplayId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long personDisplayId,
        long documentDisplayId,
        CancellationToken cancellationToken)
    {
        // TODO: Get actual user from authentication
        var deletedBy = "System";

        var result = await _documentService.DeleteAsync(personDisplayId, documentDisplayId, deletedBy, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Uploads a profile image for a person.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="file">The image file to upload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded profile image.</returns>
    [HttpPost("profile-image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProfileImageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequestSizeLimit(MaxImageSizeBytes)]
    public async Task<ActionResult<ProfileImageResponseDto>> UploadProfileImage(
        long personDisplayId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        if (file.Length > MaxImageSizeBytes)
            return BadRequest($"Image size exceeds the maximum allowed size of {MaxImageSizeBytes / (1024 * 1024)} MB.");

        if (!AllowedImageContentTypes.Contains(file.ContentType))
            return BadRequest($"File type '{file.ContentType}' is not allowed for profile images. Only JPEG and PNG are allowed.");

        // TODO: Get actual user from authentication
        var modifiedBy = "System";

        try
        {
            var dto = new UploadDocumentDto
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length
            };

            var result = await _documentService.UploadProfileImageAsync(personDisplayId, dto, modifiedBy, cancellationToken);
            if (result == null)
                return NotFound("Person not found.");

            return Ok(new ProfileImageResponseDto { ProfileImageUrl = result });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets a person's profile image.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The profile image file.</returns>
    [HttpGet("profile-image")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileImage(
        long personDisplayId,
        CancellationToken cancellationToken)
    {
        var result = await _documentService.GetProfileImageAsync(personDisplayId, cancellationToken);
        if (result == null)
            return NotFound();

        return File(result.Content, result.ContentType);
    }

    /// <summary>
    /// Deletes a person's profile image.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("profile-image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfileImage(
        long personDisplayId,
        CancellationToken cancellationToken)
    {
        // TODO: Get actual user from authentication
        var modifiedBy = "System";

        var result = await _documentService.DeleteProfileImageAsync(personDisplayId, modifiedBy, cancellationToken);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

/// <summary>
/// Response DTO for profile image upload.
/// </summary>
public class ProfileImageResponseDto
{
    /// <summary>
    /// Gets or sets the URL of the uploaded profile image.
    /// </summary>
    public string ProfileImageUrl { get; set; } = string.Empty;
}
