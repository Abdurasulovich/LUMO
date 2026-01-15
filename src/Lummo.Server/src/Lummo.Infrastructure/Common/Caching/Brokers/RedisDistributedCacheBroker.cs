using Force.DeepCloner;
using Lummo.Application.Common.Serializer;
using Lummo.Domain.Common.Caching;
using Lummo.Infrastructure.Settings;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Lummo.Infrastructure.Common.Caching.Brokers;

internal class RedisDistributedCacheBroker(IOptions<CacheSettings> cacheSettings,
    IDistributedCache distributedCache,
    IJsonSerializationSettingsProvider jsonSerializationSettingsProvider) : ICacheBroker
{

    private readonly DistributedCacheEntryOptions _entryOption = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheSettings.Value.AbsoluteExpirationInSeconds),
        SlidingExpiration = TimeSpan.FromSeconds(cacheSettings.Value.SlidingExpirationInSeconds)
    };

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    public async ValueTask SetAsync<T>(string key, T value, CacheEntryOptions? cacheEntryOptions = null, CancellationToken cancellationToken = default)
    {
        await distributedCache.SetStringAsync(
            key,
            JsonConvert.SerializeObject(value, jsonSerializationSettingsProvider.Get()),
            cancellationToken
            );
    }

    public ValueTask DeleteAsync(string key, CancellationToken? cancellationToken = null)
    {
        distributedCache.Remove(key);
        return ValueTask.CompletedTask;
    }

    public async ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await distributedCache.GetAsync(key, cancellationToken);
        return value is not null
            ? JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(value), jsonSerializationSettingsProvider.Get())
            : default;
    }

    public async ValueTask<T?> GetOrSetAsync<T>(string key, Func<Task<T>> valueFactory, CacheEntryOptions? cacheEntryOptions = null, CancellationToken cancellationToken = default)
    {
        var cacheValue = await distributedCache.GetStringAsync(key, cancellationToken);
        if (cacheValue is not null)
            return JsonConvert.DeserializeObject<T>(cacheValue, jsonSerializationSettingsProvider.Get());

        var value = await valueFactory();
        await SetAsync(key, await valueFactory(), cacheEntryOptions, cancellationToken);

        return value;
    }

    public ValueTask<bool> TryGetAsync<T>(string key, out T? value, CancellationToken cancellationToken = default)
    {
        var foundEntry = distributedCache.GetString(key);
        if(foundEntry is not null)
        {
            value = JsonConvert.DeserializeObject<T>(foundEntry, jsonSerializationSettingsProvider.Get());
            return ValueTask.FromResult(true);
        }
        value = default;

        return ValueTask.FromResult(false);
    }

    public DistributedCacheEntryOptions GetCacheEntryOptions(CacheEntryOptions? cacheEntryOptions)
    {
        if (cacheEntryOptions == default
            || (!cacheEntryOptions.AbsoluteExpirationRelativeNow.HasValue
            && !cacheEntryOptions.SlidingExpiration.HasValue))
            return _entryOption;

        var currentEntryOptions = _entryOption.DeepClone();

        currentEntryOptions.AbsoluteExpirationRelativeToNow = _entryOption.AbsoluteExpirationRelativeToNow.HasValue
            ? currentEntryOptions.AbsoluteExpirationRelativeToNow
            : null;
        currentEntryOptions.SlidingExpiration = _entryOption.SlidingExpiration.HasValue
            ? currentEntryOptions.SlidingExpiration
            : null;

        return currentEntryOptions;
    }
}
