using Lummo.Application.Common.Notifications.Models;

namespace Lummo.Application.Common.Notifications.Events;

public class SenderNotificationEvent : NotificationEvent
{
    public NotificationMessage Message { get; set; } = default!;
}
