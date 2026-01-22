using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Constants;
using Lummo.Domain.Entities;
using Lummo.Domain.Extensions;
using Lummo.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Lummo.Infrastructure.Common.Identity.Services;

public class IdentitySecurityTokenGeneratorService(IOptions<JwtSettings> jwtSettings)
    : IIdentitySecurityTokenGeneratorService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    public AccessToken GenerateAccessToken(User user)
    {
        var accessToken = new AccessToken
        {
            Id = Guid.NewGuid()
        };

        var jwtToken = GetToken(user, accessToken);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        accessToken.Token = token;

        return accessToken;
    }

    public RefreshToken GenerateRefreshTokenAsync(User user, bool extendedExpiryTime = false)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(randomNumber),
            UserId = user.Id,
            ExpiryTime = DateTime.UtcNow.AddMinutes(
                extendedExpiryTime
                ? _jwtSettings.RefreshTokenExtendedExpirationTimeInMinutes
                : _jwtSettings.RefreshTokenExpirationTimeInMinutes
            )
        };
    }

    public (AccessToken AccessToken, bool IsExpired)? GetAccessToken(string tokenValue)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var getAccessToken = () =>
        {
            var tokenWithoutPrefix = tokenValue.Replace("Bearer ", string.Empty);

            // Remove lifetime validation
            var tokenValidationParameters = _jwtSettings.MapToTokenValidationParameters();
            tokenValidationParameters.ValidateLifetime = false;

            var principal = tokenHandler.ValidateToken(tokenWithoutPrefix, tokenValidationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
                )
            ) throw new SecurityTokenException("Invalid token");

            var isExpired = jwtSecurityToken.ValidTo.ToUniversalTime() < DateTime.UtcNow;

            return (new AccessToken
            {
                Id = Guid.Parse(principal.FindFirst(JwtRegisteredClaimNames.Jti)!.Value),
                UserId = Guid.Parse(principal.FindFirst(ClaimConstants.UserId)!.Value),
                Token = tokenValue,
                ExpiryTime = jwtSecurityToken.ValidTo.ToUniversalTime()
            }, isExpired);
        };

        return getAccessToken.GetValue().Data;
    }

    public Guid GetAccessTokenId(string accessToken)
    {
        var tokenValue = accessToken.Split(' ')[1];

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenValue);

        var tokenId = token.Claims.FirstOrDefault(c=>c.Type == ClaimConstants.AccessTokenId)?.Value;

        if (string.IsNullOrEmpty(tokenId))
            throw new ArgumentException("Invalid Access token");

        return Guid.Parse(tokenId);
    }

    public List<Claim> GetClaims(User user, AccessToken accessToken)
    {
        return new List<Claim>
        {
            new(ClaimTypes.Email, user.EmailAddress),

            new(ClaimTypes.Role, user.Roles[0].Type.ToString()),

            new(ClaimConstants.UserId, user.Id.ToString()),

            new(JwtRegisteredClaimNames.Jti, accessToken.Id.ToString())
        };
    }

    public JwtSecurityToken GetToken(User user, AccessToken accessToken)
    {
        var claims = GetClaims(user, accessToken);
        accessToken.UserId = user.Id;
        accessToken.ExpiryTime = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTimeInMinutes);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: accessToken.ExpiryTime.UtcDateTime,
            signingCredentials: credentials
        );
    }
}
