using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
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

    [NotMapped]
    public EmailTemplate EmailTemplate
    {
        get => Template is not null ? Template as EmailTemplate : null;
        set => Template = value;
    }
}
