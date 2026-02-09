using Lummo.Application.Common.Identity.Models;
using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IAccountService
{
    ValueTask<User?> GetUserByEmailAddressAsync(string emailAddress,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<User> CreateUserAsync(User user,
        bool skipEmailVerification = false,
        CancellationToken cancellationToken = default);

    ValueTask<bool> VerifyUserAsync(EmailVerificationDetails code,
        CancellationToken cancellationToken = default);

    ValueTask<bool> ResendVerificationCodeAsync(string emailAddress,
        CancellationToken cancellationToken = default);
}
