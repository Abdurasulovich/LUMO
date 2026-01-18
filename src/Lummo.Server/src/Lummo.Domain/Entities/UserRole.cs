using Lummo.Domain.Common.Entities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lummo.Domain.Entities;

public class UserRole : IEntity
{
    public UserRole(){}
    public UserRole(Guid userId, Guid roleId)
        => (UserId, RoleId) = (userId, roleId);
    [NotMapped]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
