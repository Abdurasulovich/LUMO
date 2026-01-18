using Lummo.Domain.Enums;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IRoleProcessingService
{
    ValueTask GrandRoleAsync(Guid userId,
        RoleType actionUserRole,
        CancellationToken cancellationToken = default);
    ValueTask GrandRoleBySystemAsync(Guid userId, 
        RoleType roleType,
        CancellationToken cancellationToken = default);

    ValueTask RevokeRoleAsync(Guid userId,
        RoleType roleType,
        CancellationToken cancellationToken = default);
}
