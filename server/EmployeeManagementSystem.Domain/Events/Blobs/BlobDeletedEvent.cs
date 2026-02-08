namespace EmployeeManagementSystem.Domain.Events.Blobs;

public sealed class BlobDeletedEvent(
    string blobName,
    string containerName,
    string contentType,
    string relatedEntityType,
    string relatedEntityId) : DomainEvent
{
    public string BlobName { get; } = blobName;
    public string ContainerName { get; } = containerName;
    public string ContentType { get; } = contentType;
    public string RelatedEntityType { get; } = relatedEntityType;
    public string RelatedEntityId { get; } = relatedEntityId;

    public override string EventType =>
        ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
            ? "com.ems.blob.image.deleted"
            : "com.ems.blob.document.deleted";
}
