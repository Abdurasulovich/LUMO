using Lummo.Application.Common.Identity.Models;
using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IAuthService
{
    ValueTask<bool> SignUpAsync(
        SignUpDetails signUpDetails,
        CancellationToken cancellationToken = default);

    ValueTask<(AccessToken accessToken, RefreshToken refreshToken)> SignUpWithGoogleAsync(GoogleSignInRequest idToken,
        CancellationToken cancellationToken = default);

    ValueTask<(AccessToken accessToken, RefreshToken refreshToken)> SignInAsync(
        SignInDetails signInDetails,
        CancellationToken cancellationToken = default);

    ValueTask<(AccessToken accessToken, RefreshToken refreshToken)> SignInWithGoogleAsync(GoogleSignInRequest idToken,
        CancellationToken cancellationToken = default);

    ValueTask<bool> GrandRoleAsync(Guid userId,
        string roleType,
        CancellationToken cancellationToken = default);
    ValueTask<bool> RevokeRoleAsync(Guid userId,
        string roleType,
        CancellationToken cancellationToken = default);

    ValueTask<AccessToken> RefreshTokenAsync(
        string refreshTokenValue,
        CancellationToken cancellationToken = default);
}
