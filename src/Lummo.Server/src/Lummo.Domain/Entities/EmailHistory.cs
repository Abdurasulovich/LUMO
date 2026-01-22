using Type = Lummo.Domain.Enums.NotificationType;
namespace Lummo.Domain.Entities;

public class EmailHistory : NotificationHistory
{
    public EmailHistory()
    {
        Type = Type.Email;
    }

    public string SenderEmailAddress { get; set; } = default!;

    public string ReceiverEmailAddress { get; set; } = default!;

    public string Subject { get; set; } = default!;

    public EmailTemplate Template { get; set; }
}
