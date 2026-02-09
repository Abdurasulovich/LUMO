using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Services.Models;

namespace Lummo.Mobile.Platforms.iOS.Services;

public class AuthService : IAuthService
{
    public ValueTask<bool> SignInAsync(SignIn signIn, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SignInWithGoogleServiceAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<User> SignUpAsync(SignUp signUp, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> SignUpWithGoogleServiceAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
