using Lummo.Domain.Common.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Domain.Entities;

public class User : Entity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateTime BirthDate { get; set; }
    public string EmailAddress { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public bool IsEmailAddressVerified { get; set; }
    public string? ImageUrl { get; set; }
    public RoleType Role {  get; set; }
    public UserSettings? UserSettings { get; set; }
}
