using Lummo.Domain.Common.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Domain.Entities;

public class UserSettings : AuditableEntity
{
    public Theme PreferredTheme { get; set; }
    public NotificationType? PreferredNotificationType { get; set ; }
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}
