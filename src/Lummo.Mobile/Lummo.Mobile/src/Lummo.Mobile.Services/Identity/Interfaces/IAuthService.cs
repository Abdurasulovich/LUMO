using Lummo.Mobile.ApiClient.Models;
using Lummo.Mobile.Services.Models;

namespace Lummo.Mobile.Services.Identity.Interfaces;

public interface IAuthService
{
    Task<User> SignUpWithGoogleServiceAsync(
        CancellationToken cancellationToken = default);
    ValueTask<User> SignUpAsync(
        SignUp signUp,
        CancellationToken cancellationToken = default);

    Task<bool> SignInWithGoogleServiceAsync(
        CancellationToken cancellationToken = default);
    ValueTask<bool> SignInAsync(
        SignIn signIn, AuthProvider authProvider,
        CancellationToken cancellationToken = default);


}
