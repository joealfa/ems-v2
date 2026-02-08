namespace EmployeeManagementSystem.Domain.Events.Positions;

public sealed class PositionDeletedEvent(Guid positionId, string titleName) : DomainEvent
{
    public Guid PositionId { get; } = positionId;
    public string TitleName { get; } = titleName;

    public override string EventType => "com.ems.position.deleted";
}
