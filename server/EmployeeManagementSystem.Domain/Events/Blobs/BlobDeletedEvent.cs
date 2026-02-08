namespace EmployeeManagementSystem.Domain.Events.Blobs;

public sealed class BlobDeletedEvent : DomainEvent
{
    public BlobDeletedEvent(
        string blobName,
        string containerName,
        string contentType,
        string relatedEntityType,
        string relatedEntityId)
    {
        BlobName = blobName;
        ContainerName = containerName;
        ContentType = contentType;
        RelatedEntityType = relatedEntityType;
        RelatedEntityId = relatedEntityId;
    }

    public string BlobName { get; }
    public string ContainerName { get; }
    public string ContentType { get; }
    public string RelatedEntityType { get; }
    public string RelatedEntityId { get; }

    public override string EventType =>
        ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
            ? "com.ems.blob.image.deleted"
            : "com.ems.blob.document.deleted";
}
