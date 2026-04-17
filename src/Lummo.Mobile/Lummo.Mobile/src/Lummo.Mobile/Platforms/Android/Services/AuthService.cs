using Android.App;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Lummo.Mobile.ApiClient.Interfaces;
using Lummo.Mobile.ApiClient.Models;
using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Services.Interfaces;
using Lummo.Mobile.Services.Models;
using Lummo.Mobile.Views.Popups;
using Mopups.Services;

namespace Lummo.Mobile.Platforms.Android.Services;

public class AuthService : IAuthService
{
    private readonly ILummoApiClient _apiClient;
    private readonly IUserService _userService;
    private readonly ITokenResolver _tokenResolver;
    private readonly Activity _activity;
    private readonly GoogleSignInClient _googleSignInClient;
    private TaskCompletionSource<GoogleAuthResult>? _authCompletionSource;

    public AuthService(ILummoApiClient apiClient, IUserService userService, ITokenResolver tokenResolver)
    {
        _activity = Platform.CurrentActivity
            ?? throw new InvalidOperationException("Activity mavjud emas");
        var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            .RequestIdToken("608206764181-f1bj129v2u0004uc3e7jhmrjobcpbis2.apps.googleusercontent.com") // Google Cloud Console dan oling
            .RequestEmail()
            .RequestId()
            .RequestProfile()
            .Build();

        _googleSignInClient = GoogleSignIn.GetClient(_activity, gso);

        MainActivity.ResultGoogleAuth += OnGoogleAuthResult;
        _apiClient = apiClient;
        _userService = userService;
        _tokenResolver = tokenResolver;
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
                PhotoUrl = result.Account.PhotoUrl?.ToString(),
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
        _authCompletionSource = new TaskCompletionSource<GoogleAuthResult>();

        // Cancellation token bilan bog'lash
        cancellationToken.Register(() =>
        {
            _authCompletionSource?.TrySetCanceled();
        });

        await _googleSignInClient.SignOutAsync();
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

    public async Task<bool> SignUpWithGoogleServiceAsync(CancellationToken cancellationToken = default)
    {
        var result = await SignInWithGoogleAsync(cancellationToken);

            if (!result.IsSuccess)
                throw new Exception(result.ErrorMessage);

            // _apiClient.SignUpAsync returns IdentityTokenDto, not bool.
            // Await the DTO and determine success by checking for a non-null instance and a non-empty AccessToken.
            var idToken = new GoogleSignInRequest
            {
                GoogleIdToken = result.IdToken
            };
            var identityToken = await _apiClient.SignUpWithGoogleAsync(idToken, cancellationToken);
            if (identityToken != null && !string.IsNullOrEmpty(identityToken.AccessToken))
            {
                var tokenInfo = new TokenInfo
                {
                    AccessToken = identityToken.AccessToken,
                    RefreshToken = identityToken.RefreshToken,
                    AccessTokenExpiresAt = identityToken.AccessTokenExpiryTime,
                    RefreshTokenExpiresAt = identityToken.RefreshTokenExpiryTime
                };
                await _tokenResolver.ClearTokenAsync();
                await _tokenResolver.SetTokenAsync(tokenInfo);
                var userId = await _userService.GetUserIdFromAccessToken(tokenInfo.AccessToken);
                if (userId is null)
                    throw new InvalidOperationException("User ID not found in access token.");

                var user = await _apiClient.AccountsGETAsync(userId.Value, cancellationToken);
                var userInfo = new User
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.EmailAddress,
                    UserName = user.Username,
                };
                await _userService.AddUserAsync(userInfo);
                return true;
            }
            return false;
    }

    public async Task<bool> SignInWithGoogleServiceAsync(CancellationToken cancellationToken = default)
    {
        var result = await SignInWithGoogleAsync(cancellationToken);

        if (!result.IsSuccess)
            throw new Exception(result.ErrorMessage);

        var payload = new GoogleSignInRequest
        {
            GoogleIdToken = result.IdToken
        };

        var accessToken = await _apiClient.SignInWithGoogleAsync(payload, cancellationToken);
        var tokenInfo = new TokenInfo
        {
            AccessToken = accessToken.AccessToken,
            RefreshToken = accessToken.RefreshToken,
            AccessTokenExpiresAt = accessToken.AccessTokenExpiryTime,
            RefreshTokenExpiresAt = accessToken.RefreshTokenExpiryTime
        };
        await _tokenResolver.ClearTokenAsync();
        await _tokenResolver.SetTokenAsync(tokenInfo);

        var userId = await _userService.GetUserIdFromAccessToken(accessToken.AccessToken);
        if (userId is null)
            throw new InvalidOperationException("User ID not found in access token.");

        var user = await _apiClient.AccountsGETAsync(userId.Value, cancellationToken);
        await _userService.AddUserAsync(new User
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.EmailAddress,
            UserName = user.Username,
        });

        return true;
    }
    public async ValueTask<bool> SignInAsync(SignIn signIn, CancellationToken cancellationToken = default)
    {

        var payload = new SignInDetails
        {
            UsernameOrEmail = signIn.UsernameOrEmail,
            Password = signIn.Password,
            RememberMe = signIn.RememberMe,
            AuthProvider = AuthProvider.Email,
        };

        var accessToken = await _apiClient.SignInAsync(payload, cancellationToken);
        var token = new TokenInfo
        {
            AccessToken = accessToken.AccessToken,
            RefreshToken = accessToken.RefreshToken,
            AccessTokenExpiresAt = accessToken.AccessTokenExpiryTime,
            RefreshTokenExpiresAt = accessToken.RefreshTokenExpiryTime
        };

        await _tokenResolver.ClearTokenAsync();
        await _tokenResolver.SetTokenAsync(token);

        var userId = await _userService.GetUserIdFromAccessToken(accessToken.AccessToken);
        if (userId is null)
            throw new InvalidOperationException("User ID not found in access token.");

        var user = await _apiClient.AccountsGETAsync(userId.Value, cancellationToken);
        await _userService.DeleteUserAsync();
        await _userService.AddUserAsync(new User
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.EmailAddress,
            UserName = user.Username
        });

        return true;
    }

    public async ValueTask<bool> SignUpAsync(SignUp signUp, CancellationToken cancellationToken = default)
    {
        var payload = new SignUpDetails
        {
            FirstName = signUp.FirstName,
            LastName = signUp.LastName,
            UserName = signUp.UserName ?? signUp.EmailAddress.Split('@')[0],
            AuthProvider = AuthProvider.Email,
            EmailAddress = signUp.EmailAddress,
            Password = signUp.Password ?? string.Empty,
            AutoGeneratePassword = string.IsNullOrEmpty(signUp.Password),
            Age = signUp.Age,
            GoogleIdToken = string.Empty
        };

        return await _apiClient.SignUpAsync(payload, cancellationToken);
    }

    public async ValueTask<bool> VerifyEmail(EmailVerificationDetails emailVerificationDetails, CancellationToken cancellationToken = default)
    {
        try
        {
            await _apiClient.VerifyEmailAsync(new EmailVerificationDetails
            {
                EmailAddress = emailVerificationDetails.EmailAddress,
                VerificationCode = emailVerificationDetails.VerificationCode
            }, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public ValueTask<bool> ResendVerificationCode(ResendVerificationCodeRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return new ValueTask<bool>(_apiClient.ResendVerificationCodeAsync(new ResendVerificationCodeRequest
            {
                EmailAddress = request.EmailAddress
            }, cancellationToken));
        }
        catch (Exception ex)
        {
            return new ValueTask<bool>(false);
        }
    }

    public async ValueTask<bool> ForgotPasswordVerifyEmailAsync(ForgotPasswordEmailVerificationDetails emailVerificationDetails, CancellationToken cancellationToken = default)
    {
        try
        {
            await _apiClient.ForgotPasswordVerifyEmailAsync(new ForgotPasswordEmailVerificationDetails
            {
                EmailAddress = emailVerificationDetails.EmailAddress
            }, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public ValueTask<bool> ResetPasswordAsync(ResetPasswordDetails resetPasswordDetails, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
    #endregion