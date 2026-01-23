namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for Azure Blob Storage operations.
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a file to Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The container name.</param>
    /// <param name="blobName">The blob name (path).</param>
    /// <param name="content">The file content stream.</param>
    /// <param name="contentType">The content type (MIME type).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The URL of the uploaded blob.</returns>
    Task<string> UploadAsync(
        string containerName,
        string blobName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The container name.</param>
    /// <param name="blobName">The blob name (path).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The file content stream.</returns>
    Task<Stream> DownloadAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The container name.</param>
    /// <param name="blobName">The blob name (path).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully, false otherwise.</returns>
    Task<bool> DeleteAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a blob exists.
    /// </summary>
    /// <param name="containerName">The container name.</param>
    /// <param name="blobName">The blob name (path).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the blob exists, false otherwise.</returns>
    Task<bool> ExistsAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a SAS URL for temporary access to a blob.
    /// </summary>
    /// <param name="containerName">The container name.</param>
    /// <param name="blobName">The blob name (path).</param>
    /// <param name="expiresIn">How long the SAS URL should be valid.</param>
    /// <returns>The SAS URL for the blob.</returns>
    string GenerateSasUrl(
        string containerName,
        string blobName,
        TimeSpan expiresIn);
}
