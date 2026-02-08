namespace EmployeeManagementSystem.Domain.Events.Items;

public sealed class ItemUpdatedEvent : DomainEvent
{
    public ItemUpdatedEvent(Guid itemId, Dictionary<string, object?> changes)
    {
        ItemId = itemId;
        Changes = changes;
    }

    public Guid ItemId { get; }
    public Dictionary<string, object?> Changes { get; }

    public override string EventType => "com.ems.item.updated";
}
