namespace Lummo.Domain.Common.Entities.Interfaces;

public interface ICreationAuditableEntity
{
    public Guid CreatedByUserid { get; set; }
}
