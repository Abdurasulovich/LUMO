using Lummo.Domain.Common.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Domain.Entities;

public class User : AuditableEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string EmailAddress { get; set; } = default!;
    public IList<Role> Roles {  get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailAddressVerified { get; set; }
    public AuthProvider AuthProvider { get; set; } = AuthProvider.Email;
    public UserSettings? UserSettings { get; set; }
    public UserCredentials UserCredentials { get; set; }
}
