using Lummo.Domain.Enums;

namespace Lummo.Server.Models.DTOs;

public class EmailTemplateDto
{
    public Guid Id { get; set; }

    public string Subject { get; set; } = default!;

    public string Content { get; set; } = default!;

    public NotificationType Type { get; set; }

    public NotificationTemplateType TemplateType { get; set; }
}
