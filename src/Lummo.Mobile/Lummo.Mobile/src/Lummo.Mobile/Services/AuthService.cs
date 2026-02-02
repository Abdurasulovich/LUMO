#if ANDROID
using Android.App;
using Android.Gms.Auth.Api.SignIn;
using Lummo.Mobile.Core.Models;
using Lummo.Mobile.Services.Identity.Interfaces;

namespace Lummo.Mobile.Services;

public class AuthService : IAuthService
{
    private readonly Activity _activity;
    private readonly GoogleSignInClient _googleSignInClient;
    private TaskCompletionSource<GoogleAuthResult>? _authCompletionSource;

    public AuthService()
    {
        _activity = Platform.CurrentActivity
            ?? throw new InvalidOperationException("Activity mavjud emas");

        var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            .RequestIdToken("521797977895-dm0kr62lc1k2vn4lnhq10s0i5oi2k5r5.apps.googleusercontent.com") // Google Cloud Console dan oling
            .RequestEmail()
            .RequestId()
            .RequestProfile()
            .Build();

        _googleSignInClient = GoogleSignIn.GetClient(_activity, gso);

        MainActivity.ResultGoogleAuth += OnGoogleAuthResult;
    }

    private void OnGoogleAuthResult(object? sender, (bool Success, GoogleSignInAccount? Account, string? Error) result)
    {
        if (_authCompletionSource == null) return;

        if (result.Success && result.Account != null)
        {
            var authResult = new GoogleAuthResult
            {
                IsSuccess = true,
                Email = result.Account.Email ?? string.Empty,
                DisplayName = result.Account.DisplayName ?? string.Empty,
                GivenName = result.Account.GivenName ?? string.Empty,
                FamilyName = result.Account.FamilyName ?? string.Empty,
                IdToken = result.Account.IdToken ?? string.Empty,
                PhotoUrl = result.Account.PhotoUrl?.ToString()
            };

            _authCompletionSource.TrySetResult(authResult);
        }
        else
        {
            _authCompletionSource.TrySetResult(new GoogleAuthResult
            {
                IsSuccess = false,
                ErrorMessage = result.Error ?? "Noma'lum xatolik"
            });
        }
    }

    public async Task<GoogleAuthResult> SignInWithGoogleAsync(CancellationToken cancellationToken = default)
    {
        // Avval mavjud hisobni tekshirish
        var existingAccount = GoogleSignIn.GetLastSignedInAccount(_activity);
        if (existingAccount != null)
        {
            return new GoogleAuthResult
            {
                IsSuccess = true,
                Email = existingAccount.Email ?? string.Empty,
                DisplayName = existingAccount.DisplayName ?? string.Empty,
                GivenName = existingAccount.GivenName ?? string.Empty,
                FamilyName = existingAccount.FamilyName ?? string.Empty,
                IdToken = existingAccount.IdToken ?? string.Empty,
                PhotoUrl = existingAccount.PhotoUrl?.ToString()
            };
        }

        _authCompletionSource = new TaskCompletionSource<GoogleAuthResult>();

        // Cancellation token bilan bog'lash
        cancellationToken.Register(() =>
        {
            _authCompletionSource?.TrySetCanceled();
        });

        _activity.StartActivityForResult(_googleSignInClient.SignInIntent, 9001);

        return await _authCompletionSource.Task;
    }

    public async Task SignOutAsync()
    {
        await _googleSignInClient.SignOutAsync();
    }

    public async Task RevokeAccessAsync()
    {
        await _googleSignInClient.RevokeAccessAsync();
    }

    #region IAuthService implementation

    public async Task<SignUpDetails> SignUpWithGoogleAsync(SignUpDetails signUp, CancellationToken cancellationToken = default)
    {
        var result = await SignInWithGoogleAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            throw new Exception(result.ErrorMessage);
        }

        return new SignUpDetails
        {
            FirstName = result.GivenName,
            LastName = result.FamilyName,
            EmailAddress = result.Email,
            UserName = result.Email,
            AutoGeneratePassword = true
        };
    }

    public async Task<SignInDetails> SignInWithGoogleAsync(SignUpDetails signUp, CancellationToken cancellationToken = default)
    {
        var result = await SignInWithGoogleAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            throw new Exception(result.ErrorMessage);
        }

        return new SignInDetails
        {
            UsernameOrEmail = result.Email
        };
    }

    public ValueTask<bool> SignUpAsync(SignUpDetails signUp, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Oddiy ro'yxatdan o'tish hali tayyor emas");
    }

    public ValueTask<bool> SignInAsync(SignInDetails signIn, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Oddiy kirish hali tayyor emas");
    }

    #endregion
}
#endif
