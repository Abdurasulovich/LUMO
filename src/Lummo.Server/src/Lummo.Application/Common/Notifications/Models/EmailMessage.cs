using Lummo.Domain.Entities;
using System.Text.Json.Serialization;

namespace Lummo.Application.Common.Notifications.Models;

public class EmailMessage : NotificationMessage
{
    public string SenderEmailAddress { get; set; } = default!;
    public string ReceiverEmailAddress { get; set; } = default!;
    public EmailTemplate Template { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Body { get; set; }
}
