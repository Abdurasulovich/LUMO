using Lummo.Application.Common.Notifications.Models;

namespace Lummo.Application.Common.Notifications.Brokers.Interfaces;

public interface IEmailSenderBroker
{
    ValueTask<bool> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}
