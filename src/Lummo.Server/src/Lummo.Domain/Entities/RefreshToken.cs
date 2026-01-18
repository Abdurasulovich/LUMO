using Lummo.Domain.Common.Entities;

namespace Lummo.Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; set; }

    public string Token { get; set; } = default!;

    public DateTimeOffset ExpiryTime { get; set; }

    public bool EnableExpiryExtendedTime { get; set; }
}
