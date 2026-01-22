using Lummo.Domain.Common.Entities.Interfaces;

namespace Lummo.Domain.Common.Entities;

public class Entity : IEntity
{
    public Guid Id { get; set; }
}
