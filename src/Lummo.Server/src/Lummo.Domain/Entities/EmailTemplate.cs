using Type = Lummo.Domain.Enums.NotificationType;
namespace Lummo.Domain.Entities;

public class EmailTemplate : NotificationTemplate
{
    public EmailTemplate()
    {
        Type = Type.Email;
    }

    public string Subject { get; set; } = default!;
}
