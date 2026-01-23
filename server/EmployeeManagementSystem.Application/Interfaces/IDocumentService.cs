using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Document;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for document operations.
/// </summary>
public interface IDocumentService
{
    /// <summary>
    /// Gets a document by display ID for a specific person.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The document details or null if not found.</returns>
    Task<DocumentResponseDto?> GetByDisplayIdAsync(
        long personDisplayId,
        long documentDisplayId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of documents for a person.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of documents.</returns>
    Task<PagedResult<DocumentListDto>> GetPagedAsync(
        long personDisplayId,
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a document for a person.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="dto">The document upload data.</param>
    /// <param name="createdBy">The user who is uploading the document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created document details.</returns>
    Task<DocumentResponseDto?> UploadAsync(
        long personDisplayId,
        UploadDocumentDto dto,
        string createdBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates document metadata.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="dto">The update data.</param>
    /// <param name="modifiedBy">The user who is modifying the document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated document details or null if not found.</returns>
    Task<DocumentResponseDto?> UpdateAsync(
        long personDisplayId,
        long documentDisplayId,
        UpdateDocumentDto dto,
        string modifiedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a document.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The download result or null if not found.</returns>
    Task<BlobDownloadResultDto?> DownloadAsync(
        long personDisplayId,
        long documentDisplayId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="documentDisplayId">The document's display ID.</param>
    /// <param name="deletedBy">The user who is deleting the document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteAsync(
        long personDisplayId,
        long documentDisplayId,
        string deletedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a profile image for a person.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="dto">The image upload data.</param>
    /// <param name="modifiedBy">The user who is uploading the image.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded profile image or null if person not found.</returns>
    Task<string?> UploadProfileImageAsync(
        long personDisplayId,
        UploadDocumentDto dto,
        string modifiedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a person's profile image.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="modifiedBy">The user who is deleting the image.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false if not found.</returns>
    Task<bool> DeleteProfileImageAsync(
        long personDisplayId,
        string modifiedBy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a person's profile image.
    /// </summary>
    /// <param name="personDisplayId">The person's display ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The download result or null if not found.</returns>
    Task<BlobDownloadResultDto?> GetProfileImageAsync(
        long personDisplayId,
        CancellationToken cancellationToken = default);
}
