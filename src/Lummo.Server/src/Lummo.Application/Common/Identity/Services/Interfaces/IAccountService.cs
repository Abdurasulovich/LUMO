using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IAccountService
{
    ValueTask<User?> GetUserByEmailAddressAsync(string emailAddress,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<User> CreateUserAsync(User user,
        CancellationToken cancellationToken = default);

    ValueTask<bool> VerifyUserAsync(string code,
        CancellationToken cancellationToken = default);

    ValueTask<bool> ResendVerificationCodeAsync(string emailAddress,
        CancellationToken cancellationToken = default);
}
