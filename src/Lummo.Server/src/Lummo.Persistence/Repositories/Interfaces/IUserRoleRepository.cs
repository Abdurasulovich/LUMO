using Lummo.Domain.Entities;

namespace Lummo.Persistence.Repositories.Interfaces;

public interface IUserRoleRepository
{
    ValueTask<UserRole> CreateAsync(UserRole userRole,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<UserRole?> DeleteAsync(UserRole userRole,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
}
