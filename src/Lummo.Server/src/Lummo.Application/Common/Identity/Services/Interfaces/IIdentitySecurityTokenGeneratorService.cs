using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IIdentitySecurityTokenGeneratorService
{
    AccessToken GenerateAccessToken(User user);

    RefreshToken GenerateRefreshTokenAsync(User user,
        bool extendedExpiryTime = false);

    (AccessToken AccessToken, bool IsExpired)? GetAccessToken(string tokenValue);

    Guid GetAccessTokenId(string accessToken);

    JwtSecurityToken GetToken(User user, AccessToken accessToken);

    List<Claim> GetClaims(User user, AccessToken accessToken);
}
