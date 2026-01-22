using Lummo.Application.Common.EventBus.Brokers.Interfaces;
using Lummo.Application.Common.Serializer;
using Lummo.Domain.Common.Events;
using Lummo.Domain.Common.Events.Interfaces;
using Lummo.Domain.Events;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using System.Text;

namespace Lummo.Infrastructure.Common.EventBus.Brokers;

public class RabbitMqEventBusBroker(
    IRabbitMqConnectionProvider rabbitMqConnectionProvider,
    IJsonSerializationSettingsProvider jsonSerializationSettingsProvider,
    IMediator mediator)
    : IEventBusBroker
{
    public async ValueTask PublishAsync<TEvent>(TEvent @event, string exchange, string routingKey, CancellationToken cancellationToken = default)
        where TEvent : Event
    {
        var channel = await rabbitMqConnectionProvider.CreateChannelAsync();

        var properties = new BasicProperties
        {
            Persistent = true
        };

        var serializeSettings = jsonSerializationSettingsProvider.Get(true);
        serializeSettings.ContractResolver = new DefaultContractResolver();
        serializeSettings.TypeNameHandling = TypeNameHandling.All;

        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, serializeSettings));
        await channel.BasicPublishAsync(exchange: exchange, routingKey: routingKey, false, basicProperties: properties, body: body);
    }

    public ValueTask PublishLocalAsync<TEvent>(TEvent command) where TEvent : IEvent
        => new ValueTask(mediator.Publish(command));
}
