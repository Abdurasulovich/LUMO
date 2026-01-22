using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.Caching.Models;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<User, AppDbContext>(dbContext, cacheBroker, new CacheEntryOptions())
    , IUserRepository
{
    public new IQueryable<User> Get(Expression<Func<User, bool>>? predicate, bool asNoTracking)
        => base.Get(predicate, asNoTracking);

    public new ValueTask<User?> GetByIdAsync(Guid userId, bool asNoTracking, CancellationToken cancellationToken)
        => base.GetByIdAsync(userId, asNoTracking, cancellationToken);

    public new ValueTask<IList<User>> GetByIdsAsync(IEnumerable<Guid> ids, bool asNoTracking, CancellationToken cancellationToken)
        => base.GetByIdsAsync(ids, asNoTracking, cancellationToken);

    public new ValueTask<User> CreateAsync(User user, bool saveChanges, CancellationToken cancellationToken)
        => base.CreateAsync(user, saveChanges, cancellationToken);

    public new ValueTask<User> UpdateAsync(User user, bool saveChanges, CancellationToken cancellationToken)
        => base.UpdateAsync(user, saveChanges, cancellationToken);

    public new ValueTask<bool> DeleteAsync(User user, bool saveChanges, CancellationToken cancellationToken)
        => base.DeleteAsync(user, saveChanges, cancellationToken);

    public new ValueTask<bool> DeleteByIdAsync(Guid userId, bool saveChanges, CancellationToken cancellationToken)
        => base.DeleteByIdAsync(userId, saveChanges, cancellationToken);
}
