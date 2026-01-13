using Lummo.Domain.Entities;

namespace Lummo.Persistence.Repositories.Interfaces;

public interface IAccessTokenRepository
{
    ValueTask<AccessToken> CreateAsync(AccessToken accessToken,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<AccessToken?> GetByIdAsync(Guid accessTokenId,
        CancellationToken cancellationToken = default);

    ValueTask<AccessToken> UpdateAsync(AccessToken accessToken,
        CancellationToken cancellationToken = default);
}
