using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Notifications.Events;

public class RenderNotificationEvent : NotificationEvent
{
    public NotificationTemplate Template { get; set; } = default!;

    public User SenderUser { get; set; } = default!;

    public User ReceiverUser { get; set; } = default!;

    public Dictionary<string, string> Variables { get; set; } = new();
}
