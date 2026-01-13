using Lummo.Domain.Common.Caching;
using Lummo.Domain.Common.Entities.Interfaces;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Persistence.Repositories;

public class EntityRepositoryBase<TEntity, TContext>(
    TContext dbContext,
    ICacheBorker cacheBorker,
    CacheEntryOptions? cacheEntryOptions = default)
    where TEntity : class, IEntity where TContext : DbContext
{
}
