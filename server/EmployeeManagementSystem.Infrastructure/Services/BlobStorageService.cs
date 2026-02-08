using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using EmployeeManagementSystem.Application.Events;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Events.Blobs;
using EmployeeManagementSystem.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EmployeeManagementSystem.Infrastructure.Services;

/// <summary>
/// Azure Blob Storage service implementation.
/// </summary>
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _connectionString;
    private readonly IEventPublisher _eventPublisher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<BlobStorageService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobStorageService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="eventPublisher">The event publisher for domain events.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor for metadata.</param>
    /// <param name="logger">The logger instance.</param>
    public BlobStorageService(
        IConfiguration configuration,
        IEventPublisher eventPublisher,
        IHttpContextAccessor httpContextAccessor,
        ILogger<BlobStorageService> logger)
    {
        _connectionString = configuration.GetConnectionString("BlobStorage")
            ?? throw new InvalidOperationException("BlobStorage connection string is not configured.");
        _blobServiceClient = new BlobServiceClient(_connectionString);
        _eventPublisher = eventPublisher;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(
        string containerName,
        string blobName,
        Stream content,
        string contentType,
        string? relatedEntityType = null,
        string? relatedEntityId = null,
        CancellationToken cancellationToken = default)
    {
        long sizeBytes = content.Length;
        _logger.LogIfEnabled(LogLevel.Debug, "Uploading blob {BlobName} to container {ContainerName}, Size: {Size} bytes, ContentType: {ContentType}",
            blobName, containerName, sizeBytes, contentType);

        try
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            _ = await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            BlobHttpHeaders blobHttpHeaders = new()
            {
                ContentType = contentType
            };

            _ = await blobClient.UploadAsync(content, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            }, cancellationToken);

            string url = blobClient.Uri.ToString();

            _logger.LogIfEnabled(LogLevel.Information, "Blob uploaded successfully: {BlobName} in container {ContainerName}", blobName, containerName);

            // Publish domain event if entity context is provided
            if (!string.IsNullOrEmpty(relatedEntityType) && !string.IsNullOrEmpty(relatedEntityId))
            {
                await PublishBlobUploadedEventAsync(
                    blobName,
                    containerName,
                    contentType,
                    sizeBytes,
                    url,
                    relatedEntityType,
                    relatedEntityId,
                    cancellationToken);
            }

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogIfEnabled(LogLevel.Error, ex, "Failed to upload blob {BlobName} to container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogIfEnabled(LogLevel.Debug, "Downloading blob {BlobName} from container {ContainerName}", blobName, containerName);

        try
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            Response<BlobDownloadStreamingResult> response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

            _logger.LogIfEnabled(LogLevel.Information, "Blob downloaded successfully: {BlobName} from container {ContainerName}", blobName, containerName);

            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogIfEnabled(LogLevel.Error, ex, "Failed to download blob {BlobName} from container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        string containerName,
        string blobName,
        string? contentType = null,
        string? relatedEntityType = null,
        string? relatedEntityId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogIfEnabled(LogLevel.Debug, "Deleting blob {BlobName} from container {ContainerName}", blobName, containerName);

        try
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            Response<bool> response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            if (response.Value)
            {
                _logger.LogIfEnabled(LogLevel.Information, "Blob deleted successfully: {BlobName} from container {ContainerName}", blobName, containerName);

                // Publish domain event if entity context is provided
                if (!string.IsNullOrEmpty(contentType) && !string.IsNullOrEmpty(relatedEntityType) && !string.IsNullOrEmpty(relatedEntityId))
                {
                    await PublishBlobDeletedEventAsync(
                        blobName,
                        containerName,
                        contentType,
                        relatedEntityType,
                        relatedEntityId,
                        cancellationToken);
                }
            }
            else
            {
                _logger.LogIfEnabled(LogLevel.Warning, "Blob not found for deletion: {BlobName} in container {ContainerName}", blobName, containerName);
            }

            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogIfEnabled(LogLevel.Error, ex, "Failed to delete blob {BlobName} from container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        Response<bool> response = await blobClient.ExistsAsync(cancellationToken);
        return response.Value;
    }

    /// <inheritdoc />
    public string GenerateSasUrl(
        string containerName,
        string blobName,
        TimeSpan expiresIn)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        if (!blobClient.CanGenerateSasUri)
        {
            // If using connection string, we can generate SAS
            // For managed identity, you need to use user delegation SAS
            throw new InvalidOperationException("Cannot generate SAS URI. Ensure the connection string includes account key.");
        }

        BlobSasBuilder sasBuilder = new()
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiresIn)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }

    #region Event Publishing Helpers

    private async Task PublishBlobUploadedEventAsync(
        string blobName,
        string containerName,
        string contentType,
        long sizeBytes,
        string url,
        string relatedEntityType,
        string relatedEntityId,
        CancellationToken cancellationToken)
    {
        try
        {
            BlobUploadedEvent domainEvent = new(
                blobName: blobName,
                containerName: containerName,
                contentType: contentType,
                sizeBytes: sizeBytes,
                url: url,
                relatedEntityType: relatedEntityType,
                relatedEntityId: relatedEntityId
            );

            EventMetadata metadata = CreateEventMetadata();
            string userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

            await _eventPublisher.PublishAsync(
                domainEvent,
                userId,
                _httpContextAccessor.HttpContext?.TraceIdentifier,
                metadata,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogIfEnabled(LogLevel.Error, ex, "Failed to publish BlobUploadedEvent for {BlobName}", blobName);
        }
    }

    private async Task PublishBlobDeletedEventAsync(
        string blobName,
        string containerName,
        string contentType,
        string relatedEntityType,
        string relatedEntityId,
        CancellationToken cancellationToken)
    {
        try
        {
            BlobDeletedEvent domainEvent = new(
                blobName: blobName,
                containerName: containerName,
                contentType: contentType,
                relatedEntityType: relatedEntityType,
                relatedEntityId: relatedEntityId
            );

            EventMetadata metadata = CreateEventMetadata();
            string userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

            await _eventPublisher.PublishAsync(
                domainEvent,
                userId,
                _httpContextAccessor.HttpContext?.TraceIdentifier,
                metadata,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogIfEnabled(LogLevel.Error, ex, "Failed to publish BlobDeletedEvent for {BlobName}", blobName);
        }
    }

    private EventMetadata CreateEventMetadata()
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext;
        return httpContext == null
            ? new EventMetadata()
            : new EventMetadata
            {
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                Source = "BlobStorageService"
            };
    }

    #endregion
}
