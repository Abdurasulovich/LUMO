using Lummo.Domain.Common.EventBus.Brokers.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Lummo.Infrastructure.Common.EventBus.Services;

public abstract class EventBusBackgroundService(IEnumerable<IEventSubscriber> eventSubscribers) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.WhenAll(eventSubscribers.Select(eventSubscriber =>
        eventSubscriber.StartAsync(stoppingToken).AsTask()));
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(eventSubscribers.Select(eventSubscriber =>
        eventSubscriber.StopAsync(cancellationToken).AsTask()));
    }
}
