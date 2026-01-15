namespace Lummo.Domain.Common.EventBus.Brokers.Interfaces;

public interface IEventSubscriber
{
    ValueTask StartAsync(CancellationToken cancellationToken = default);

    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
