using Lummo.Application.Common.EventBus.Brokers.Interfaces;
using Lummo.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Lummo.Infrastructure.Common.EventBus.Brokers;

public class RabbitMqConnectionProvider(IOptions<RabbitMqConnectionSettings> rabbitMqConnectionSettings)
    : IRabbitMqConnectionProvider
{
    private readonly ConnectionFactory _connectionFactory = new()
    {
        HostName = rabbitMqConnectionSettings.Value.HostName,
        Port = rabbitMqConnectionSettings.Value.Port
    };

    private IConnection? _connection;
    public async ValueTask<IChannel> CreateChannelAsync()
    {
        _connection ??= await _connectionFactory.CreateConnectionAsync();

        return await _connection.CreateChannelAsync();
    }
}
