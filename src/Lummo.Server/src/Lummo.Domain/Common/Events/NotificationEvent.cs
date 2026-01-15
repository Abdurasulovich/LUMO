namespace Lummo.Domain.Common.Events;

public class NotificationEvent : Event
{
    public Guid SenderUserId { get; init; }

    public Guid ReceiverUserId { get; init; }
}
