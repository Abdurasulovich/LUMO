using FluentValidation;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Entities;
using Lummo.Persistence.Repositories.Interfaces;

namespace Lummo.Infrastructure.Common.Identity.Services;

public class IdentitySecurityTokenService(
    IAccessTokenRepository accessTokenRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IValidator<RefreshToken> refreshTokenValidator)
    : IIdentitySecurityTokenService
{
    public ValueTask<AccessToken> CreateAccessTokenAsync(AccessToken accessToken, bool saveChanges = true, CancellationToken cancellationToken = default)
    => accessTokenRepository.CreateAsync(accessToken, saveChanges, cancellationToken);

    public ValueTask<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken, bool saveChanges = true, CancellationToken cancellation = default)
    {
        var validationResult = refreshTokenValidator.Validate(refreshToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return refreshTokenRepository.CreateAsync(refreshToken, saveChanges, cancellation);
    }

    public ValueTask<AccessToken?> GetAccessTokenByIdAsync(Guid accessTokenId, CancellationToken cancellationToken = default)
    => accessTokenRepository.GetByIdAsync(accessTokenId, cancellationToken);

    public ValueTask<RefreshToken?> GetRefreshTokenByValueAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    => refreshTokenRepository.GetByValueAsync(refreshTokenValue, cancellationToken);

    public async ValueTask RemoveAccessTokenAsync(Guid accessTokenId, CancellationToken cancelToken = default)
    =>await accessTokenRepository.DeleteByIdAsync(accessTokenId, cancelToken);

    public ValueTask RemoveRefreshTokenAsync(string refreshTokenValue, CancellationToken cancelToken = default)
    =>refreshTokenRepository.RemoveAsync(refreshTokenValue, cancelToken);

    public async ValueTask RevokeAccessTokenAsync(Guid accessTokenId, CancellationToken cancellationToken = default)
    {
        var accessToken = await GetAccessTokenByIdAsync(accessTokenId, cancellationToken);
        if (accessToken is null)
            throw new InvalidOperationException($"Access with id {accessTokenId} not found.");

        accessToken.IsRevoked = true;
        await accessTokenRepository.UpdateAsync(accessToken, cancellationToken);
    }
}
