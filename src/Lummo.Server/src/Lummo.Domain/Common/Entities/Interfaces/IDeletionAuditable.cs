namespace Lummo.Domain.Common.Entities.Interfaces;

public interface IDeletionAuditable
{
    public Guid? DeletedByUserId { get; set; }
}
