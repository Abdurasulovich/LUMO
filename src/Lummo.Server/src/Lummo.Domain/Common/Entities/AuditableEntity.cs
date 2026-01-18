using Lummo.Domain.Common.Entities.Interfaces;

namespace Lummo.Domain.Common.Entities;

public abstract class AuditableEntity : SoftDeletedEntity, IAuditableEntity
{
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset? ModifiedTime { get; set; }
}
