namespace EmployeeManagementSystem.Domain.Events.Positions;

public sealed class PositionUpdatedEvent(Guid positionId, Dictionary<string, object?> changes) : DomainEvent
{
    public Guid PositionId { get; } = positionId;
    public Dictionary<string, object?> Changes { get; } = changes;

    public override string EventType => "com.ems.position.updated";
}
