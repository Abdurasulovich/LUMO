using Lummo.Domain.Common.Caching;
using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.Repositories.Interfaces;

namespace Lummo.Persistence.Repositories;

public class AccessTokenRepository : IAccessTokenRepository
{
    private readonly ICacheBorker _cacheBroker;

    public AccessTokenRepository(ICacheBorker cacheBroker)
    {
        _cacheBroker = cacheBroker;
    }
    public async ValueTask<AccessToken> CreateAsync(AccessToken accessToken, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var cacheEntryOptions = new CacheEntryOptions(accessToken.ExpiryTime - DateTimeOffset.UtcNow, null);
        await _cacheBroker.SetAsync(accessToken.Id.ToString(), accessToken, cacheEntryOptions, cancellationToken);

        return accessToken;
    }

    public ValueTask<AccessToken?> GetByIdAsync(Guid accessTokenId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<AccessToken> UpdateAsync(AccessToken accessToken, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
