using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Document;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Services;

/// <summary>
/// Service implementation for document operations.
/// </summary>
public class DocumentService : IDocumentService
{
    private readonly IRepository<Document> _documentRepository;
    private readonly IRepository<Person> _personRepository;
    private readonly IBlobStorageService _blobStorageService;

    private const string DocumentsContainer = "documents";
    private const string ProfileImagesContainer = "profile-images";

    private static readonly Dictionary<string, DocumentType> ExtensionToDocumentType = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".pdf", DocumentType.Pdf },
        { ".doc", DocumentType.Word },
        { ".docx", DocumentType.Word },
        { ".xls", DocumentType.Excel },
        { ".xlsx", DocumentType.Excel },
        { ".ppt", DocumentType.PowerPoint },
        { ".pptx", DocumentType.PowerPoint },
        { ".jpg", DocumentType.ImageJpeg },
        { ".jpeg", DocumentType.ImageJpeg },
        { ".png", DocumentType.ImagePng }
    };

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".jpg", ".jpeg", ".png"
    };

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentService"/> class.
    /// </summary>
    public DocumentService(
        IRepository<Document> documentRepository,
        IRepository<Person> personRepository,
        IBlobStorageService blobStorageService)
    {
        _documentRepository = documentRepository;
        _personRepository = personRepository;
        _blobStorageService = blobStorageService;
    }

    /// <inheritdoc />
    public async Task<Result<DocumentResponseDto>> GetByDisplayIdAsync(
        long personDisplayId,
        long documentDisplayId,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .FirstOrDefaultAsync(p => p.DisplayId == personDisplayId, cancellationToken);

        if (person == null)
            return Result<DocumentResponseDto>.NotFound("Person not found.");

        var document = await _documentRepository.Query()
            .FirstOrDefaultAsync(d => d.PersonId == person.Id && d.DisplayId == documentDisplayId, cancellationToken);

        if (document == null)
            return Result<DocumentResponseDto>.NotFound("Document not found.");

        return Result<DocumentResponseDto>.Success(MapToResponseDto(document, personDisplayId));
    }

    /// <inheritdoc />
    public async Task<PagedResult<DocumentListDto>> GetPagedAsync(
        long personDisplayId,
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .FirstOrDefaultAsync(p => p.DisplayId == personDisplayId, cancellationToken);

        if (person == null)
        {
            return new PagedResult<DocumentListDto>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        var queryable = _documentRepository.Query()
            .Where(d => d.PersonId == person.Id);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.ToLower();
            queryable = queryable.Where(d =>
                d.FileName.ToLower().Contains(searchTerm) ||
                (d.Description != null && d.Description.ToLower().Contains(searchTerm)));
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        queryable = query.SortDescending
            ? queryable.OrderByDescending(d => d.CreatedOn)
            : queryable.OrderBy(d => d.CreatedOn);

        var items = await queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => new DocumentListDto
            {
                DisplayId = d.DisplayId,
                FileName = d.FileName,
                FileExtension = d.FileExtension,
                ContentType = d.ContentType,
                FileSizeBytes = d.FileSizeBytes,
                DocumentType = d.DocumentType,
                Description = d.Description,
                CreatedOn = d.CreatedOn,
                CreatedBy = d.CreatedBy
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<DocumentListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<Result<DocumentResponseDto>> UploadAsync(
        long personDisplayId,
        UploadDocumentDto dto,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .FirstOrDefaultAsync(p => p.DisplayId == personDisplayId, cancellationToken);

        if (person == null)
            return Result<DocumentResponseDto>.NotFound("Person not found.");

        var extension = Path.GetExtension(dto.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            return Result<DocumentResponseDto>.BadRequest($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}");
        }

        var documentType = GetDocumentType(extension);
        var blobName = GenerateBlobName(person.Id, dto.FileName);

        var blobUrl = await _blobStorageService.UploadAsync(
            DocumentsContainer,
            blobName,
            dto.FileStream,
            dto.ContentType,
            cancellationToken);

        var document = new Document
        {
            FileName = dto.FileName,
            FileExtension = extension,
            ContentType = dto.ContentType,
            FileSizeBytes = dto.FileSizeBytes,
            DocumentType = documentType,
            BlobUrl = blobUrl,
            BlobName = blobName,
            ContainerName = DocumentsContainer,
            Description = dto.Description,
            PersonId = person.Id,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow
        };

        await _documentRepository.AddAsync(document, cancellationToken);

        return Result<DocumentResponseDto>.Success(MapToResponseDto(document, personDisplayId));
    }

    /// <inheritdoc />
    public async Task<Result<DocumentResponseDto>> UpdateAsync(
        long personDisplayId,
        long documentDisplayId,
        UpdateDocumentDto dto,
        string modifiedBy,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .FirstOrDefaultAsync(p => p.DisplayId == personDisplayId, cancellationToken);

        if (person == null)
            return Result<DocumentResponseDto>.NotFound("Person not found.");

        var document = await _documentRepository.Query()
            .FirstOrDefaultAsync(d => d.PersonId == person.Id && d.DisplayId == documentDisplayId, cancellationToken);

        if (document == null)
            return Result<DocumentResponseDto>.NotFound("Document not found.");

        document.Description = dto.Description;
        document.ModifiedBy = modifiedBy;
        document.ModifiedOn = DateTime.UtcNow;

        await _documentRepository.UpdateAsync(document, cancellationToken);

        return Result<DocumentResponseDto>.Success(MapToResponseDto(document, personDisplayId));
    }

    /// <inheritdoc />
    public async Task<Result<BlobDownloadResultDto>> DownloadAsync(
        long personDisplayId,
        long documentDisplayId,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .FirstOrDefaultAsync(p => p.DisplayId == personDisplayId, cancellationToken);

        if (person == null)
            return Result<BlobDownloadResultDto>.NotFound("Person not found.");

        var document = await _documentRepository.Query()
            .FirstOrDefaultAsync(d => d.PersonId == person.Id && d.DisplayId == documentDisplayId, cancellationToken);

        if (document == null)
            return Result<BlobDownloadResultDto>.NotFound("Document not found.");

        var content = await _blobStorageService.DownloadAsync(
            document.ContainerName,
            document.BlobName,
            cancellationToken);

        return Result<BlobDownloadResultDto>.Success(new BlobDownloadResultDto
        {
            Content = content,
            ContentType = document.ContentType,
            FileName = document.FileName
        });
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(
        long personDisplayId,
        long documentDisplayId,
        string deletedBy,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .FirstOrDefaultAsync(p => p.DisplayId == personDisplayId, cancellationToken);

        if (person == null)
            return Result.NotFound("Person not found.");

        var document = await _documentRepository.Query()
            .FirstOrDefaultAsync(d => d.PersonId == person.Id && d.DisplayId == documentDisplayId, cancellationToken);

        if (document == null)
            return Result.NotFound("Document not found.");

        // Note: We keep the blob in storage for potential recovery
        // If you want to delete the blob as well, uncomment the line below
        // await _blobStorageService.DeleteAsync(document.ContainerName, document.BlobName, cancellationToken);

        // Soft delete the document record
        document.ModifiedBy = deletedBy;
        document.ModifiedOn = DateTime.UtcNow;
        await _documentRepository.DeleteAsync(document, cancellationToken);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<string>> UploadProfileImageAsync(
        long personDisplayId,
        UploadDocumentDto dto,
        string modifiedBy,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .FirstOrDefaultAsync(p => p.DisplayId == personDisplayId, cancellationToken);

        if (person == null)
            return Result<string>.NotFound("Person not found.");

        var extension = Path.GetExtension(dto.FileName);
        if (!AllowedImageExtensions.Contains(extension))
        {
            return Result<string>.BadRequest($"File extension '{extension}' is not allowed for profile images. Allowed extensions: {string.Join(", ", AllowedImageExtensions)}");
        }

        // Delete existing profile image if any
        if (!string.IsNullOrEmpty(person.ProfileImageUrl))
        {
            var existingBlobName = $"{person.Id}/profile{Path.GetExtension(person.ProfileImageUrl)}";
            await _blobStorageService.DeleteAsync(ProfileImagesContainer, existingBlobName, cancellationToken);
        }

        var blobName = $"{person.Id}/profile{extension}";

        var blobUrl = await _blobStorageService.UploadAsync(
            ProfileImagesContainer,
            blobName,
            dto.FileStream,
            dto.ContentType,
            cancellationToken);

        person.ProfileImageUrl = blobUrl;
        person.ModifiedBy = modifiedBy;
        person.ModifiedOn = DateTime.UtcNow;

        await _personRepository.UpdateAsync(person, cancellationToken);

        return Result<string>.Success(blobUrl);
    }

    /// <inheritdoc />
    public async Task<Result> DeleteProfileImageAsync(
        long personDisplayId,
        string modifiedBy,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .FirstOrDefaultAsync(p => p.DisplayId == personDisplayId, cancellationToken);

        if (person == null)
            return Result.NotFound("Person not found.");

        if (string.IsNullOrEmpty(person.ProfileImageUrl))
            return Result.NotFound("No profile image to delete.");

        var extension = Path.GetExtension(person.ProfileImageUrl);
        var blobName = $"{person.Id}/profile{extension}";

        await _blobStorageService.DeleteAsync(ProfileImagesContainer, blobName, cancellationToken);

        person.ProfileImageUrl = null;
        person.ModifiedBy = modifiedBy;
        person.ModifiedOn = DateTime.UtcNow;

        await _personRepository.UpdateAsync(person, cancellationToken);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<BlobDownloadResultDto>> GetProfileImageAsync(
        long personDisplayId,
        CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.Query()
            .FirstOrDefaultAsync(p => p.DisplayId == personDisplayId, cancellationToken);

        if (person == null)
            return Result<BlobDownloadResultDto>.NotFound("Person not found.");

        if (string.IsNullOrEmpty(person.ProfileImageUrl))
            return Result<BlobDownloadResultDto>.NotFound("No profile image found.");

        var extension = Path.GetExtension(person.ProfileImageUrl);
        var blobName = $"{person.Id}/profile{extension}";

        var content = await _blobStorageService.DownloadAsync(ProfileImagesContainer, blobName, cancellationToken);

        var contentType = extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };

        return Result<BlobDownloadResultDto>.Success(new BlobDownloadResultDto
        {
            Content = content,
            ContentType = contentType,
            FileName = $"profile{extension}"
        });
    }

    private static DocumentType GetDocumentType(string extension)
    {
        return ExtensionToDocumentType.TryGetValue(extension, out var type)
            ? type
            : DocumentType.Other;
    }

    private static string GenerateBlobName(Guid personId, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueId = Guid.NewGuid().ToString("N");
        return $"{personId}/{uniqueId}{extension}";
    }

    private static DocumentResponseDto MapToResponseDto(Document document, long personDisplayId)
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
}
