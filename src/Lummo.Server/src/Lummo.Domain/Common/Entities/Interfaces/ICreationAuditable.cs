namespace Lummo.Domain.Common.Entities.Interfaces;

public interface ICreationAuditable
{
    public Guid CreatedByUserid { get; set; }
}
