using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using EmployeeManagementSystem.Application.Interfaces;
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
    private readonly ILogger<BlobStorageService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobStorageService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger instance.</param>
    public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
    {
        _connectionString = configuration.GetConnectionString("BlobStorage")
            ?? throw new InvalidOperationException("BlobStorage connection string is not configured.");
        _blobServiceClient = new BlobServiceClient(_connectionString);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(
        string containerName,
        string blobName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Uploading blob {BlobName} to container {ContainerName}, Size: {Size} bytes, ContentType: {ContentType}",
            blobName, containerName, content.Length, contentType);

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

            _logger.LogInformation("Blob uploaded successfully: {BlobName} in container {ContainerName}", blobName, containerName);

            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload blob {BlobName} to container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Downloading blob {BlobName} from container {ContainerName}", blobName, containerName);

        try
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            Response<BlobDownloadStreamingResult> response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("Blob downloaded successfully: {BlobName} from container {ContainerName}", blobName, containerName);

            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download blob {BlobName} from container {ContainerName}", blobName, containerName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting blob {BlobName} from container {ContainerName}", blobName, containerName);

        try
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            Response<bool> response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            if (response.Value)
            {
                _logger.LogInformation("Blob deleted successfully: {BlobName} from container {ContainerName}", blobName, containerName);
            }
            else
            {
                _logger.LogWarning("Blob not found for deletion: {BlobName} in container {ContainerName}", blobName, containerName);
            }

            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete blob {BlobName} from container {ContainerName}", blobName, containerName);
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
}
