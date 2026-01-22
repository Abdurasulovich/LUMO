using Lummo.Domain.Entities;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories.Interfaces;

public interface IRoleRepository
{
    IQueryable<Role> Get(Expression<Func<Role, bool>>? predicate = default,
        bool asNoTracking = false);
}
