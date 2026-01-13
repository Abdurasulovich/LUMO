using Lummo.Domain.Common.Caching;
using System.ComponentModel;

namespace Lummo.Persistence.Caching.Brokers.Interfaces;

public interface ICacheBorker
{
    ValueTask<T?> GetAsync<T>(string key,
        CancellationToken cancellationToken = default);

    ValueTask<bool> TryGetAsync<T>(string key, out T? value,
        CancellationToken cancellationToken = default);

    ValueTask<T?> GetOrSetAsync<T>(string key,
        Func<Task<T>> valueFactory,
        CacheEntryOptions? cacheEntryOptions = default,
        CancellationToken cancellationToken = default);

    ValueTask SetAsync<T>(string key, T value,
        CacheEntryOptions? cacheEntryOptions = default,
        CancellationToken cancellationToken = default);

    ValueTask DeleteAsync(string key,
        CancellationToken? cancellationToken = default);
}
