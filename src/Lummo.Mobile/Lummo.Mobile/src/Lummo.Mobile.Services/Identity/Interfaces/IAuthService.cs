using Lummo.Mobile.Core.Models;

namespace Lummo.Mobile.Services.Identity.Interfaces;

public interface IAuthService
{
    Task<SignUpDetails> SignUpWithGoogleAsync(
        SignUpDetails signUp,
        CancellationToken cancellationToken = default);
    ValueTask<bool> SignUpAsync(
        SignUpDetails signUp,
        CancellationToken cancellationToken = default);

    Task<SignInDetails> SignInWithGoogleAsync(
        SignUpDetails signUp,
        CancellationToken cancellationToken = default);
    ValueTask<bool> SignInAsync(
        SignInDetails signIn,
        CancellationToken cancellationToken = default);


}
