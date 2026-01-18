using Lummo.Application.Common.EventBus.Brokers.Interfaces;
using Lummo.Application.Common.Serializer;
using Lummo.Domain.Common.EventBus.Brokers.Interfaces;
using Lummo.Domain.Common.Events;
using Lummo.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Lummo.Infrastructure.Common.EventBus.Services;

public abstract class EventSubscriber<TEvent> : IEventSubscriber where TEvent : Event
{
    private readonly EventBusSubscriberSettings _eventBusSubscriberSettings;
    private readonly JsonSerializerSettings _jsonSerializerSettings;
    private readonly IEnumerable<string> _queueNames;
    private readonly IRabbitMqConnectionProvider _rabbitMqConnectionProvider;
    private IEnumerable<AsyncEventingBasicConsumer> _consumer = default!;
    protected IChannel Channel = default!;

    protected EventSubscriber(
        IRabbitMqConnectionProvider rabbitMqConnectionProvider,
        IOptions<EventBusSubscriberSettings> eventBusSubscriberSettings,
        IEnumerable<string> queueName,
        IJsonSerializationSettingsProvider jsonSerializationSettingsProvider)
    {
        _eventBusSubscriberSettings = eventBusSubscriberSettings.Value;
        _rabbitMqConnectionProvider = rabbitMqConnectionProvider;
        _queueNames = queueName;

        _jsonSerializerSettings = jsonSerializationSettingsProvider.Get(true);
        _jsonSerializerSettings.ContractResolver = new DefaultContractResolver();
        _jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;
    }

    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        await SetChannelAsync();
        await SetConsumerAsync(cancellationToken);
    }

    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        Channel.Dispose();
        return ValueTask.CompletedTask;
    }

    protected virtual async ValueTask SetChannelAsync()
    {
        Channel = await _rabbitMqConnectionProvider.CreateChannelAsync();
        await Channel.BasicQosAsync(0, _eventBusSubscriberSettings.PerfetchCount, false);
    }

    protected virtual async ValueTask SetConsumerAsync(CancellationToken cancellationToken = default)
    {
        _consumer = await Task.WhenAll(
            _queueNames.Select(
                async queueName =>
                {
                    var consumer = new AsyncEventingBasicConsumer(Channel);
                    consumer.ReceivedAsync += async (sender, args) =>
                    await HandleInternalAsync(sender, args, cancellationToken);

                    await Channel.BasicConsumeAsync(queueName, false, consumer);

                    return consumer;
                }
                )
            );
    }

    protected virtual async ValueTask HandleInternalAsync(object? sender, BasicDeliverEventArgs ea,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var @event = JsonConvert.DeserializeObject<TEvent>(message, _jsonSerializerSettings)!;
            @event.Redelivered = ea.Redelivered;

            var result = await ProcessAsync(@event, cancellationToken);
            if (result.Result)
                await Channel.BasicAckAsync(ea.DeliveryTag, false);
            else 
                await Channel.BasicNackAsync(ea.DeliveryTag, false, result.Result);
        }
        catch
        {
            await Channel.BasicNackAsync(ea.DeliveryTag, false, true);
        }
    }

    protected abstract ValueTask<(bool Result, bool Redeliver)> ProcessAsync(TEvent @event,
        CancellationToken cancellationToken);
}
