using Lummo.Domain.Common.Events;
using Lummo.Domain.Common.Events.Interfaces;
using Lummo.Domain.Events;

namespace Lummo.Application.Common.EventBus.Brokers.Interfaces;

public interface IEventBusBroker
{
    ValueTask PublishLocalAsync<TEvent>(TEvent command) where TEvent : IEvent;
    ValueTask PublishAsync<TEvent>(TEvent @event, string exchange, string routingKey,
        CancellationToken cancellationToken = default) where TEvent : Event;
}
