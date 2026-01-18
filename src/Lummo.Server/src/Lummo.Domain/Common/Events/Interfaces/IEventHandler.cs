using MediatR;

namespace Lummo.Domain.Common.Events.Interfaces;

public interface IEventHandler<in TEvent> : IEventHandler, INotificationHandler<TEvent>
    where TEvent : IEvent
{
}

public interface IEventHandler { }
