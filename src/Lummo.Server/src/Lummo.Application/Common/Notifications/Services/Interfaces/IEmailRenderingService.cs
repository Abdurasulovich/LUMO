using Lummo.Application.Common.Notifications.Models;

namespace Lummo.Application.Common.Notifications.Services.Interfaces;

public interface IEmailRenderingService
{
    ValueTask<string> RenderAsync(
        EmailMessage emailMessage,
        CancellationToken cancellationToken = default);
}
