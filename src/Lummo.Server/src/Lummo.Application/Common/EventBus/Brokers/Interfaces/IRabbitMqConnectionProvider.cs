using RabbitMQ.Client;

namespace Lummo.Application.Common.EventBus.Brokers.Interfaces;

public interface IRabbitMqConnectionProvider
{
    ValueTask<IChannel> CreateChannelAsync();
}
