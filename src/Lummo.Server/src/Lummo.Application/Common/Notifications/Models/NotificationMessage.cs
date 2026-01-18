using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Notifications.Models;

public abstract class NotificationMessage
{
    public Guid SenderUserId { get; set; }
    public Guid ReceiverUserId { get; set; }
    public Dictionary<string, string> Variables { get; set; } = default!;

    public bool IsSuccessful { get; set; }

    public string? ErrorMessage { get; set; }
}
