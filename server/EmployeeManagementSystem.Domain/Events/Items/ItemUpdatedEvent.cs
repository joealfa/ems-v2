namespace EmployeeManagementSystem.Domain.Events.Items;

public sealed class ItemUpdatedEvent(Guid itemId, Dictionary<string, object?> changes) : DomainEvent
{
    public Guid ItemId { get; } = itemId;
    public Dictionary<string, object?> Changes { get; } = changes;

    public override string EventType => "com.ems.item.updated";
}
