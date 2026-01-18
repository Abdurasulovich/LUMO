using Lummo.Domain.Entities;
using System.Net.Http.Headers;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IIdentitySecurityTokenService
{
    ValueTask<AccessToken> CreateAccessTokenAsync(
        AccessToken accessToken,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<AccessToken?> GetAccessTokenByIdAsync(
        Guid accessTokenId,
        CancellationToken cancellationToken = default);

    ValueTask<RefreshToken> CreateRefreshTokenAsync(
        RefreshToken refreshToken,
        bool saveChanges = true,
        CancellationToken cancellation = default);

    ValueTask<RefreshToken?> GetRefreshTokenByValueAsync(
        string refreshTokenValue,
        CancellationToken cancellationToken = default);

    ValueTask RevokeAccessTokenAsync(
        Guid accessTokenId,
        CancellationToken cancellationToken = default);

    ValueTask RemoveAccessTokenAsync(
        Guid accessTokenId, 
        CancellationToken cancelToken = default);

    ValueTask RemoveRefreshTokenAsync(
        string refreshTokenValue,
        CancellationToken cancelToken = default);
}
