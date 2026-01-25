using EmployeeManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents a document associated with a person.
/// </summary>
public class Document : BaseEntity
{
    /// <summary>
    /// Gets or sets the original file name.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file extension (e.g., .pdf, .docx).
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type (MIME type) of the file.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    [Required]
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets the type of document.
    /// </summary>
    [Required]
    public DocumentType DocumentType { get; set; }

    /// <summary>
    /// Gets or sets the Azure Blob Storage URL for the document.
    /// </summary>
    [Required]
    [MaxLength(2048)]
    public string BlobUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the blob name (path) in Azure Storage.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string BlobName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the container name in Azure Blob Storage.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ContainerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description for the document.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the ID of the person this document belongs to.
    /// </summary>
    [Required]
    public Guid PersonId { get; set; }

    /// <summary>
    /// Gets or sets the person this document belongs to.
    /// </summary>
    public virtual Person Person { get; set; } = null!;
}
