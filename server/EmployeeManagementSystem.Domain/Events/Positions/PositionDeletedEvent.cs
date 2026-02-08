namespace EmployeeManagementSystem.Domain.Events.Positions;

public sealed class PositionDeletedEvent : DomainEvent
{
    public PositionDeletedEvent(Guid positionId, string titleName)
    {
        PositionId = positionId;
        TitleName = titleName;
    }

    public Guid PositionId { get; }
    public string TitleName { get; }

    public override string EventType => "com.ems.position.deleted";
}
