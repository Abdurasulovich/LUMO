using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;

namespace Lummo.Persistence.Repositories;

public class UserRoleRepository(AppDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<UserRole, AppDbContext>(dbContext, cacheBroker)
    , IUserRoleRepository
{
    public new ValueTask<UserRole> CreateAsync(UserRole userRole, bool saveChanges, CancellationToken cancellationToken)
        => base.CreateAsync(userRole, saveChanges, cancellationToken);

    public new ValueTask<bool> DeleteAsync(UserRole userRole, bool saveChanges, CancellationToken cancellationToken)
        => base.DeleteAsync(userRole, saveChanges, cancellationToken);
}
