using Lummo.Domain.Common.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Domain.Entities;

public abstract class NotificationTemplate : AuditableEntity
{
    public NotificationType Type { get; set; }

    public NotificationTemplateType TemplateType { get; set; }

    public string Content { get; set; } = default!;
}
