using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories;

public class RoleRepository(AppDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<Role, AppDbContext>(dbContext, cacheBroker), IRoleRepository
{
    public IQueryable<Role> Get(Expression<Func<Role, bool>>? predicate = null, bool asNoTracking = false)
    => base.Get(predicate, asNoTracking);
}