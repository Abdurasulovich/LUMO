namespace Lummo.Domain.Common.Entities.Interfaces;

public interface ISoftDeleteEntity : IEntity
{
    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedTime { get; set; }
}
