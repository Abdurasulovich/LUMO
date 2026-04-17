using Lummo.Mobile.ApiClient.Models;
using Lummo.Mobile.Services.Models;

namespace Lummo.Mobile.Services.Identity.Interfaces;

public interface IAuthService
{
    Task<bool> SignUpWithGoogleServiceAsync(
        CancellationToken cancellationToken = default);
    ValueTask<bool> SignUpAsync(
        SignUp signUp,
        CancellationToken cancellationToken = default);

    Task<bool> SignInWithGoogleServiceAsync(
        CancellationToken cancellationToken = default);
    ValueTask<bool> SignInAsync(
        SignIn signIn,
        CancellationToken cancellationToken = default);

    ValueTask<bool> VerifyEmail(
        EmailVerificationDetails emailVerificationDetails, 
        CancellationToken cancellationToken = default);

    ValueTask<bool> ForgotPasswordVerifyEmailAsync(
        ForgotPasswordEmailVerificationDetails emailVerificationDetails,
        CancellationToken cancellationToken = default);
    ValueTask<bool> ResetPasswordAsync(
        ResetPasswordDetails resetPasswordDetails,
        CancellationToken cancellationToken = default);
    ValueTask<bool> ResendVerificationCode(
        ResendVerificationCodeRequest request,
        CancellationToken cancellationToken = default);
}
