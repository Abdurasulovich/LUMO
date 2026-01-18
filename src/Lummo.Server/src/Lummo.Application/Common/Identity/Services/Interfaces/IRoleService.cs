using Lummo.Domain.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IRoleService
{
    ValueTask<Role?> GetByTypeAsync(
        RoleType roleType,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);
}
