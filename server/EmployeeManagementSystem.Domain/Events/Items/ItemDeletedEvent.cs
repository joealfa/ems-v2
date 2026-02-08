namespace EmployeeManagementSystem.Domain.Events.Items;

public sealed class ItemDeletedEvent(Guid itemId, string itemName) : DomainEvent
{
    public Guid ItemId { get; } = itemId;
    public string ItemName { get; } = itemName;

    public override string EventType => "com.ems.item.deleted";
}
