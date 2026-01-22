using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.Caching.Models;
using Lummo.Persistence.Repositories.Interfaces;

namespace Lummo.Persistence.Repositories;

public class RefreshTokenRepository(ICacheBroker cacheBroker) : IRefreshTokenRepository
{
    public async ValueTask<RefreshToken> CreateAsync(RefreshToken refreshToken, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var cacheEntryOptions = new CacheEntryOptions(null, refreshToken.ExpiryTime - DateTimeOffset.UtcNow);
        await cacheBroker.SetAsync($"{nameof(RefreshToken)}-{refreshToken.Token}", refreshToken, cacheEntryOptions, cancellationToken);

        return refreshToken;
    }

    public ValueTask<RefreshToken?> GetByValueAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    => cacheBroker.GetAsync<RefreshToken>($"{nameof(RefreshToken)}-{refreshTokenValue}", cancellationToken: cancellationToken);

    public ValueTask RemoveAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    => cacheBroker.DeleteAsync($"{nameof(RefreshToken)}-{refreshTokenValue}", cancellationToken: cancellationToken);
}
