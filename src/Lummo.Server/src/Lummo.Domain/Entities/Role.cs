using Lummo.Domain.Common.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Domain.Entities;

public class Role : AuditableEntity
{
    public RoleType Type { get; set; }
    public bool IsDisable { get; set; }
    public IList<User> Users { get; set; }
}
