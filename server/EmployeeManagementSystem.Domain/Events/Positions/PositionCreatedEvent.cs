namespace EmployeeManagementSystem.Domain.Events.Positions;

public sealed class PositionCreatedEvent : DomainEvent
{
    public PositionCreatedEvent(
        Guid positionId,
        string titleName,
        string? description,
        bool isActive)
    {
        PositionId = positionId;
        TitleName = titleName;
        Description = description;
        IsActive = isActive;
    }

    public Guid PositionId { get; }
    public string TitleName { get; }
    public string? Description { get; }
    public bool IsActive { get; }

    public override string EventType => "com.ems.position.created";
}
