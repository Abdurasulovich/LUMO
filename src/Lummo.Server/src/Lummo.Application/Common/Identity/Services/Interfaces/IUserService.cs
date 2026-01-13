using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IUserService
{
    IQueryable<User> Get(Expression<Func<User, bool>>? predicate = default, bool asNoTracking = false);

    ValueTask<IList<User>> GetAsync(QuerySpecification<User> querySpecification,
        CancellationToken cancellationToken = default);

    ValueTask<User?> GetByIdAsync(Guid userId, bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<User> GetSystemUserAsync(bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<Guid?> GetIdByEmailAddressAsync(string emailAddress,
        CancellationToken cancellationToken = default);

    ValueTask<User> CreateAsync(User user, bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<User> UpdateAsync(User user, bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<string> UploadImageAsync(Guid id, IFormFile imagePath, string webRootPath,
        CancellationToken cancellationToken = default);
}
