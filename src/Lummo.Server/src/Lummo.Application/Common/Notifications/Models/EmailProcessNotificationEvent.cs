using Lummo.Domain.Common.Events;
using Lummo.Domain.Enums;

namespace Lummo.Application.Common.Notifications.Models;

public class EmailProcessNotificationEvent : ProcessNotificationEvent
{
    public EmailProcessNotificationEvent()
    {
        Type = NotificationType.Email;
    }
}
