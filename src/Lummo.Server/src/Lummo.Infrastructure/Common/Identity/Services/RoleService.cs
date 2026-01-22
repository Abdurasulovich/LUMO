using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Infrastructure.Common.Identity.Services;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public async ValueTask<Role?> GetByTypeAsync(RoleType roleType, bool asNoTracking = false, CancellationToken cancellationToken = default)
        => await roleRepository.Get(asNoTracking: asNoTracking)
        .FirstOrDefaultAsync(role=>role.Type == roleType, cancellationToken);
}