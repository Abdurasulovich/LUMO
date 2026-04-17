using Lummo.Mobile.ApiClient.Models;
using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Services.Models;

namespace Lummo.Mobile.Platforms.iOS.Services;

public class AuthService : IAuthService
{
    public ValueTask<bool> ResendVerificationCode(ResendVerificationCodeRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> SignInAsync(SignIn signIn, AuthProvider authProvider, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

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

    public Task<string> SignUpWithGoogleServiceAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> VerifyEmail(EmailVerificationDetails emailVerificationDetails, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    ValueTask<bool> IAuthService.SignUpAsync(SignUp signUp, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<bool> IAuthService.SignUpWithGoogleServiceAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
