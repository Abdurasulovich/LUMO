using Lummo.Domain.Common.Entities.Interfaces;
using Lummo.Domain.Enums;

namespace Lummo.Domain.Entities;

public class UserSettings : IEntity
{
    public Guid Id { get; set ; }
    public NotificationType? PreferredNotificationType { get; set ; }
}
