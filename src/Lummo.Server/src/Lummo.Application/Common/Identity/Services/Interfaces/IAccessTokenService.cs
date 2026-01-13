using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IAccessTokenService
{
    ValueTask<AccessToken> CreateAsync(AccessToken accessToken, bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<AccessToken?> GetByIdAsync(Guid accessTokenId, CancellationToken cancellationToken = default);

    ValueTask RevokeAsync(Guid accessTokenId, CancellationToken cancellationToken = default);
}
