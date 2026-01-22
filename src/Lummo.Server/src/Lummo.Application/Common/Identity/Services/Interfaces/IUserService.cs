using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IUserService
{
    IQueryable<User> Get(Expression<Func<User, bool>>? predicate = default, bool asNoTracking = false);
    ValueTask<User?> GetByIdAsync(Guid userId, bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<User?> GetByEmailAddressAsync(string emailAddress,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<IList<User>> GetByIdsAsync(IEnumerable<Guid> ids,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<User> CreateAsync(User user, bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<User> UpdateAsync(User user, bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<bool> DeleteByIdAsync(Guid userId, 
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
    ValueTask<bool> DeleteAsync(User user,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
}
