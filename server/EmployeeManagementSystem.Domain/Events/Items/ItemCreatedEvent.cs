namespace EmployeeManagementSystem.Domain.Events.Items;

public sealed class ItemCreatedEvent : DomainEvent
{
    public ItemCreatedEvent(Guid itemId, string itemName, string? description, bool isActive)
    {
        ItemId = itemId;
        ItemName = itemName;
        Description = description;
        IsActive = isActive;
    }

    public Guid ItemId { get; }
    public string ItemName { get; }
    public string? Description { get; }
    public bool IsActive { get; }

    public override string EventType => "com.ems.item.created";
}
