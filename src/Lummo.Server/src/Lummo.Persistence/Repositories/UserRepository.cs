using System.Linq.Expressions;
using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;

namespace Lummo.Persistence.Repositories;

public class UserRepository(IdentityDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<User, IdentityDbContext>(dbContext, cacheBroker), IUserRepository
{
    ValueTask<User> IUserRepository.CreateAsync(User user, bool saveChanges, CancellationToken cancellationToken)
    {
        return CreateAsync(user, saveChanges, cancellationToken);
    }

    IQueryable<User> IUserRepository.Get(Expression<Func<User, bool>>? predicate, bool asNoTracking)
    {
        return Get(predicate, asNoTracking);
    }

    ValueTask<IList<User>> IUserRepository.GetAsync(QuerySpecification<User> querySpecification, CancellationToken cancellationToken)
    {
        return GetAsync(querySpecification, cancellationToken);
    }

    ValueTask<User?> IUserRepository.GetByIdAsync(Guid userId, bool asNoTracking, CancellationToken cancellationToken)
    {
        return GetByIdAsync(userId, asNoTracking, cancellationToken);
    }

    ValueTask<User> IUserRepository.UpdateAsync(User user, bool saveChanges, CancellationToken cancellationToken)
    {
        return UpdateAsync(user, saveChanges, cancellationToken);
    }
}
