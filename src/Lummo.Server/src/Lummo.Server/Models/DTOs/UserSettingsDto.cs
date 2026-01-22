using Lummo.Domain.Enums;

namespace Lummo.Server.Models.DTOs;

public class UserSettingsDto
{
    public Guid Id { get; set; }

    public Theme PreferredTheme { get; set; }

    public NotificationType PreferredNotificationType { get; set; }

    public Guid UserId { get; set; }
}
