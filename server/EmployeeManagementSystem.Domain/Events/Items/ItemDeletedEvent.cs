namespace EmployeeManagementSystem.Domain.Events.Items;

public sealed class ItemDeletedEvent : DomainEvent
{
    public ItemDeletedEvent(Guid itemId, string itemName)
    {
        ItemId = itemId;
        ItemName = itemName;
    }

    public Guid ItemId { get; }
    public string ItemName { get; }

    public override string EventType => "com.ems.item.deleted";
}
