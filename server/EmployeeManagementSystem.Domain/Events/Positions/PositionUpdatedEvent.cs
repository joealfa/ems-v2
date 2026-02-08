namespace EmployeeManagementSystem.Domain.Events.Positions;

public sealed class PositionUpdatedEvent : DomainEvent
{
    public PositionUpdatedEvent(Guid positionId, Dictionary<string, object?> changes)
    {
        PositionId = positionId;
        Changes = changes;
    }

    public Guid PositionId { get; }
    public Dictionary<string, object?> Changes { get; }

    public override string EventType => "com.ems.position.updated";
}
