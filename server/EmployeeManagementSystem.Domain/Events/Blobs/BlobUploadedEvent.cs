namespace EmployeeManagementSystem.Domain.Events.Blobs;

public sealed class BlobUploadedEvent : DomainEvent
{
    public BlobUploadedEvent(
        string blobName,
        string containerName,
        string contentType,
        long sizeBytes,
        string url,
        string relatedEntityType,
        string relatedEntityId)
    {
        BlobName = blobName;
        ContainerName = containerName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        Url = url;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;
    }

    public string BlobName { get; }
    public string ContainerName { get; }
    public string ContentType { get; }
    public long SizeBytes { get; }
    public string Url { get; }
    public string RelatedEntityType { get; }
    public string RelatedEntityId { get; }

    public override string EventType =>
        ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
            ? "com.ems.blob.image.uploaded"
            : "com.ems.blob.document.uploaded";
}
