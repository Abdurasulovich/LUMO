namespace Lummo.Domain.Common.Entities.Interfaces;

public interface IModificationAuditable
{
    public Guid? ModifiedByUserId { get; set; }
}
