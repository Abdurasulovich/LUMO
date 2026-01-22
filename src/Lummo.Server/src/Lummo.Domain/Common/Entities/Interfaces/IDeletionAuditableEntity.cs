namespace Lummo.Domain.Common.Entities.Interfaces;

public interface IDeletionAuditableEntity
{
    public Guid? DeletedByUserId { get; set; }
}
