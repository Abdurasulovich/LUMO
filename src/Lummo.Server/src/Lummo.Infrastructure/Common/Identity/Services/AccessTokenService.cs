using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Entities;
using Lummo.Persistence.Repositories.Interfaces;

namespace Lummo.Infrastructure.Common.Identity.Services;

public class AccessTokenService(IAccessTokenRepository accessTokenRepository) : IAccessTokenService
{
    public ValueTask<AccessToken> CreateAsync(AccessToken accessToken, bool saveChanges = true, CancellationToken cancellationToken = default)
    => accessTokenRepository.CreateAsync(accessToken, saveChanges, cancellationToken);

    public ValueTask<AccessToken?> GetByIdAsync(Guid accessTokenId, CancellationToken cancellationToken = default)
    => accessTokenRepository.GetByIdAsync(accessTokenId, cancellationToken);

    public async ValueTask RevokeAsync(Guid accessTokenId, CancellationToken cancellationToken = default)
    {
        var accessToken = await GetByIdAsync(accessTokenId, cancellationToken);
        if(accessToken is null)
            throw new InvalidOperationException($"Access token with id {accessTokenId} not found");

        accessToken.IsRevoked = true;
        await accessTokenRepository.UpdateAsync(accessToken, cancellationToken);
    }
}
