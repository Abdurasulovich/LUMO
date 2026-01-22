using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Lummo.Infrastructure.Common.Settings;

public class JwtSettings
{
    public bool ValidateIssuer { get; set; }

    public string ValidIssuer { get; set; } = default!;
    public bool ValidateAudience { get; set; }

    public string ValidAudience { get; set; } = default!;
    public bool ValidateLifetime { get; set; }
    public int ExpirationTimeInMinutes { get; set; }

    public bool ValidateIssuerSigningKey { get; set; }

    public string SecretKey { get; set; } = default!;

    public int RefreshTokenExpirationTimeInMinutes { get; set; }

    public int RefreshTokenExtendedExpirationTimeInMinutes { get; set; }

    public TokenValidationParameters MapToTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = ValidateIssuer,
            ValidIssuer = ValidIssuer,
            ValidateAudience = ValidateAudience,
            ValidAudience = ValidAudience,
            ValidateLifetime = ValidateLifetime,
            ValidateIssuerSigningKey = ValidateIssuerSigningKey,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey))
        };
    }
}
