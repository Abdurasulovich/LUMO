using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories.Interfaces;

public interface IUserRepository
{
    IQueryable<User> Get(Expression<Func<User, bool>>? predicate = default,
        bool asNoTracking = false);

    ValueTask<IList<User>> GetAsync(QuerySpecification<User> querySpecification,
        CancellationToken cancellationToken = default);

    ValueTask<User?> GetByIdAsync(Guid userId,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<User> CreateAsync(User user,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<User> UpdateAsync(User user,
        bool saveChanges = true,
        CancellationToken cancellationToken);
}
