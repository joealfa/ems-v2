namespace EmployeeManagementSystem.Domain.Events.Blobs;

public sealed class BlobUploadedEvent(
    string blobName,
    string containerName,
    string contentType,
    long sizeBytes,
    string url,
    string relatedEntityType,
    string relatedEntityId) : DomainEvent
{
    public string BlobName { get; } = blobName;
    public string ContainerName { get; } = containerName;
    public string ContentType { get; } = contentType;
    public long SizeBytes { get; } = sizeBytes;
    public string Url { get; } = url;
    public string RelatedEntityType { get; } = relatedEntityType;
    public string RelatedEntityId { get; } = relatedEntityId;

    public override string EventType =>
        ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
            ? "com.ems.blob.image.uploaded"
            : "com.ems.blob.document.uploaded";
}
