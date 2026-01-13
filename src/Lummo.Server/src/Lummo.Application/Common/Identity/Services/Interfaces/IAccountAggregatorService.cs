using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IAccountAggregatorService
{
    ValueTask<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default);
}
