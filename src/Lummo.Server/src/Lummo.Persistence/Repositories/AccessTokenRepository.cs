using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.Caching.Models;
using Lummo.Persistence.Repositories.Interfaces;

namespace Lummo.Persistence.Repositories;

public class AccessTokenRepository(ICacheBroker cacheBroker) : IAccessTokenRepository
{
    public async ValueTask<AccessToken> CreateAsync(AccessToken accessToken, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var cacheEntryOptions = new CacheEntryOptions(accessToken.ExpiryTime - DateTimeOffset.UtcNow, null);
        await cacheBroker.SetAsync(accessToken.Id.ToString(), accessToken, cacheEntryOptions, cancellationToken);

        return accessToken;
    }

    public async ValueTask<AccessToken?> DeleteByIdAsync(Guid accessTokenId, CancellationToken cancellationToken = default)
    {
        var foundAccessToken = await cacheBroker.GetAsync<AccessToken>(accessTokenId.ToString(), cancellationToken);
        await cacheBroker.DeleteAsync(accessTokenId.ToString(), cancellationToken);

        return foundAccessToken;
    }

    public async ValueTask<AccessToken?> GetByIdAsync(Guid accessTokenId, CancellationToken cancellationToken = default)
    {
        return await cacheBroker.GetAsync<AccessToken>(accessTokenId.ToString(), cancellationToken);
    }

    public async ValueTask<AccessToken> UpdateAsync(AccessToken accessToken, CancellationToken cancellationToken = default)
    {
        var cacheEntryOptions = new CacheEntryOptions(accessToken.ExpiryTime - DateTimeOffset.UtcNow, null);
        await cacheBroker.SetAsync(accessToken.Id.ToString(), accessToken, cacheEntryOptions, cancellationToken);

        return accessToken;
    }
}
