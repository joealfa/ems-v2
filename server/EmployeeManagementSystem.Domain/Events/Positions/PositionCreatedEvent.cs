namespace EmployeeManagementSystem.Domain.Events.Positions;

public sealed class PositionCreatedEvent(
    Guid positionId,
    string titleName,
    string? description,
    bool isActive) : DomainEvent
{
    public Guid PositionId { get; } = positionId;
    public string TitleName { get; } = titleName;
    public string? Description { get; } = description;
    public bool IsActive { get; } = isActive;

    public override string EventType => "com.ems.position.created";
}
