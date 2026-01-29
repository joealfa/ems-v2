using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Application.DTOs.Document;

/// <summary>
/// Record for document list items. Immutable by design.
/// </summary>
public record DocumentListDto
{
    /// <summary>
    /// Gets the display ID of the document.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets the original file name.
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the file extension.
    /// </summary>
    public string FileExtension { get; init; } = string.Empty;

    /// <summary>
    /// Gets the content type (MIME type).
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    public long FileSizeBytes { get; init; }

    /// <summary>
    /// Gets the document type.
    /// </summary>
    public DocumentType DocumentType { get; init; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets when the document was created.
    /// </summary>
    public DateTime CreatedOn { get; init; }

    /// <summary>
    /// Gets who created the document.
    /// </summary>
    public string CreatedBy { get; init; } = string.Empty;
}

/// <summary>
/// Record for document response with full details. Immutable by design.
/// </summary>
public record DocumentResponseDto : DocumentListDto
{
    /// <summary>
    /// Gets the blob URL for downloading the document.
    /// </summary>
    public string BlobUrl { get; init; } = string.Empty;

    /// <summary>
    /// Gets the person's display ID this document belongs to.
    /// </summary>
    public long PersonDisplayId { get; init; }

    /// <summary>
    /// Gets the date and time when the document was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; init; }

    /// <summary>
    /// Gets who last modified the document.
    /// </summary>
    public string? ModifiedBy { get; init; }
}

/// <summary>
/// DTO for uploading a document. Kept as class due to required Stream property.
/// </summary>
public class UploadDocumentDto
{
    /// <summary>
    /// Gets or sets the file to upload.
    /// </summary>
    public required Stream FileStream { get; set; }

    /// <summary>
    /// Gets or sets the original file name.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Gets or sets the content type (MIME type).
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public required long FileSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets an optional description.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating document metadata.
/// </summary>
public class UpdateDocumentDto
{
    /// <summary>
    /// Gets or sets the updated description.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// DTO for blob download result. Kept as class due to required Stream property.
/// </summary>
public class BlobDownloadResultDto
{
    /// <summary>
    /// Gets or sets the file content stream.
    /// </summary>
    public required Stream Content { get; set; }

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    public required string FileName { get; set; }
}
