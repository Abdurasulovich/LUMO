using Lummo.Application.Common.Serializer;
using Lummo.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Lummo.Infrastructure.Common.Caching.Brokers;

internal class RedisDistributedCacheBroker(IOptions<CacheSettings> cacheSettings,
    IDistributedCache distributedCache,
    IJsonSerializationSettingsProvider jsonSerializationSettingsProvider)
{
}
