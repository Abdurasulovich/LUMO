namespace Lummo.Server.Models.DTOs;

public class IdentityTokenDto
{
    public string AccessToken { get; set; } = default!;

    public string RefreshToken { get; set; } = default!;

    public DateTimeOffset AccessTokenExpiryTime { get; set; }

    public DateTimeOffset RefreshTokenExpiryTime { get; set; }
}
