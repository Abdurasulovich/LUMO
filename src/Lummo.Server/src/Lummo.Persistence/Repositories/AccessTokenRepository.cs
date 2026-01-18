using Lummo.Domain.Common.Caching;
using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Lummo.Persistence.Repositories;

public class AccessTokenRepository : IAccessTokenRepository
{
    private readonly ICacheBroker _cacheBroker;

    public AccessTokenRepository(ICacheBroker cacheBroker)
    {
        _cacheBroker = cacheBroker;
    }
    public async ValueTask<AccessToken> CreateAsync(AccessToken accessToken, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var cacheEntryOptions = new CacheEntryOptions(accessToken.ExpiryTime - DateTimeOffset.UtcNow, null);
        await _cacheBroker.SetAsync(accessToken.Id.ToString(), accessToken, cacheEntryOptions, cancellationToken);

        return accessToken;
    }

    public async ValueTask<AccessToken?> DeleteByIdAsync(Guid accessTokenId, CancellationToken cancellationToken = default)
    {
        var foundAccessToken = await _cacheBroker.GetAsync<AccessToken>(accessTokenId.ToString(), cancellationToken);
        await _cacheBroker.DeleteAsync(accessTokenId.ToString(), cancellationToken);

        return foundAccessToken;
    }

    public async ValueTask<AccessToken?> GetByIdAsync(Guid accessTokenId, CancellationToken cancellationToken = default)
    {
        return await _cacheBroker.GetAsync<AccessToken>(accessTokenId.ToString(), cancellationToken);
    }

    public async ValueTask<AccessToken> UpdateAsync(AccessToken accessToken, CancellationToken cancellationToken = default)
    {
        var cacheEntryOptions = new CacheEntryOptions(accessToken.ExpiryTime - DateTimeOffset.UtcNow, null);
        await _cacheBroker.SetAsync(accessToken.Id.ToString(), accessToken, cacheEntryOptions, cancellationToken);

        return accessToken;
    }
}
