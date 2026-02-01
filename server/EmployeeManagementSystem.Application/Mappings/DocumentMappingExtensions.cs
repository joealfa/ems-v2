using EmployeeManagementSystem.Application.DTOs.Document;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Document entities to DTOs.
/// </summary>
public static class DocumentMappingExtensions
{
    /// <summary>
    /// Extension method for document.
    /// </summary>
    /// <param name="document">The document entity to map.</param>
    extension(Document document)
    {
        /// <summary>
        /// Maps a Document entity to a DocumentResponseDto.
        /// </summary>
        /// <param name="personDisplayId">The display ID of the associated person.</param>
        public DocumentResponseDto ToResponseDto(long personDisplayId)
        {
            return new DocumentResponseDto
            {
                DisplayId = document.DisplayId,
                FileName = document.FileName,
                FileExtension = document.FileExtension,
                ContentType = document.ContentType,
                FileSizeBytes = document.FileSizeBytes,
                DocumentType = document.DocumentType,
                Description = document.Description,
                BlobUrl = document.BlobUrl,
                PersonDisplayId = personDisplayId,
                CreatedOn = document.CreatedOn,
                CreatedBy = document.CreatedBy,
                ModifiedOn = document.ModifiedOn,
                ModifiedBy = document.ModifiedBy
            };
        }

        /// <summary>
        /// Maps a Document entity to a DocumentListDto.
        /// </summary>
        /// <returns>The mapped DocumentListDto.</returns>
        public DocumentListDto ToListDto()
        {
            return new DocumentListDto
            {
                DisplayId = document.DisplayId,
                FileName = document.FileName,
                FileExtension = document.FileExtension,
                ContentType = document.ContentType,
                FileSizeBytes = document.FileSizeBytes,
                DocumentType = document.DocumentType,
                Description = document.Description,
                CreatedOn = document.CreatedOn,
                CreatedBy = document.CreatedBy
            };
        }
    }
}
