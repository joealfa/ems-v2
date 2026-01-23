using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Application.DTOs.Document;

/// <summary>
/// DTO for document list items.
/// </summary>
public class DocumentListDto
{
    /// <summary>
    /// Gets or sets the display ID of the document.
    /// </summary>
    public long DisplayId { get; set; }

    /// <summary>
    /// Gets or sets the original file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file extension.
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type (MIME type).
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the document type.
    /// </summary>
    public DocumentType DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets when the document was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets who created the document.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// DTO for document response with full details.
/// </summary>
public class DocumentResponseDto : DocumentListDto
{
    /// <summary>
    /// Gets or sets the blob URL for downloading the document.
    /// </summary>
    public string BlobUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the person's display ID this document belongs to.
    /// </summary>
    public long PersonDisplayId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the document was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// Gets or sets who last modified the document.
    /// </summary>
    public string? ModifiedBy { get; set; }
}

/// <summary>
/// DTO for uploading a document.
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
/// DTO for blob download result.
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
