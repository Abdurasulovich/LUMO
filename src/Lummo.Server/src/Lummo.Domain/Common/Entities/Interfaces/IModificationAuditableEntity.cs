namespace Lummo.Domain.Common.Entities.Interfaces;

public interface IModificationAuditableEntity
{
    public Guid? ModifiedByUserId { get; set; }
}
