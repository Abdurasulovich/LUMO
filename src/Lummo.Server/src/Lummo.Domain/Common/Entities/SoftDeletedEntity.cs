using Lummo.Domain.Common.Entities.Interfaces;

namespace Lummo.Domain.Common.Entities;

public class SoftDeletedEntity : Entity, ISoftDeleteEntity
{
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedTime { get; set; }
}
