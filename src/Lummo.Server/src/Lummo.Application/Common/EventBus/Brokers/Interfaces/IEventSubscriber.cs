namespace Lummo.Application.Common.EventBus.Brokers.Interfaces;

public interface IEventSubscriber
{
    ValueTask StartAsync(CancellationToken cancellationToken);
    ValueTask StopAsync(CancellationToken cancellationToken);
}
