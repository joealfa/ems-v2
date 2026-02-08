namespace EmployeeManagementSystem.Domain.Events.Items;

public sealed class ItemCreatedEvent(Guid itemId, string itemName, string? description, bool isActive) : DomainEvent
{
    public Guid ItemId { get; } = itemId;
    public string ItemName { get; } = itemName;
    public string? Description { get; } = description;
    public bool IsActive { get; } = isActive;

    public override string EventType => "com.ems.item.created";
}
