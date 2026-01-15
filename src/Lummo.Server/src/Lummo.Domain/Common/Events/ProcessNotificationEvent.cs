using Lummo.Domain.Enums;

namespace Lummo.Domain.Common.Events;

public class ProcessNotificationEvent : NotificationEvent
{
    public NotificationTemplateType TemplateType { get; init; }

    public NotificationType? Type { get; set; }

    public Dictionary<string, string>? Variables { get; set; }
}
