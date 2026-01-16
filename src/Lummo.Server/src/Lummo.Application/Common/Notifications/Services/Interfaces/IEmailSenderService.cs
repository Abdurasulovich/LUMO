using Lummo.Application.Common.Notifications.Models;

namespace Lummo.Application.Common.Notifications.Services.Interfaces;

public interface IEmailSenderService
{
    ValueTask<bool> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}
