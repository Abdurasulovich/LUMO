using Lummo.Domain.Common.Events;

namespace Lummo.Application.Common.EventBus.Brokers.Interfaces;

public interface IEventBusBroker
{
    ValueTask PublishAsync<TEvent>(TEvent @event, string exchange, string routingKey,
        CancellationToken cancellationToken = default) where TEvent : Event;
}
