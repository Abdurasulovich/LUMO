using Android.App;
using Android.Gms.Auth.Api.SignIn;
using Lummo.Mobile.ApiClient.Exceptions;
using Lummo.Mobile.ApiClient.Interfaces;
using Lummo.Mobile.ApiClient.Models;
using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Services.Models;
using Xamarin.KotlinX.Coroutines;

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
            .RequestIdToken("521797977895-dm0kr62lc1k2vn4lnhq10s0i5oi2k5r5.apps.googleusercontent.com") // Google Cloud Console dan oling
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

    public async Task<User> SignUpWithGoogleServiceAsync(CancellationToken cancellationToken = default)
    {
        var result = await SignInWithGoogleAsync(cancellationToken);

        if (!result.IsSuccess)
            throw new Exception(result.ErrorMessage);

        var payload = new SignUpDetails
        {
            FirstName = result.GivenName,
            LastName = result.FamilyName,
            EmailAddress = result.Email,
            UserName = result.Email,
            AutoGeneratePassword = true
        };

        await _apiClient.SignUpAsync(payload, cancellationToken);

        return new User
        {
            FirstName = payload.FirstName,
            LastName = payload.LastName,
            Email = payload.EmailAddress,
            UserName = payload.UserName
        };
    }

    public async Task<bool> SignInWithGoogleServiceAsync(CancellationToken cancellationToken = default)
    {
        var result = await SignInWithGoogleAsync(cancellationToken);

        if (!result.IsSuccess)
            throw new Exception(result.ErrorMessage);

        var payload = new SignInDetails
        {
            UsernameOrEmail = result.Email,
            RememberMe = true
        };

        var accessToken = await _apiClient.SignInAsync(payload, cancellationToken);
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
    public async ValueTask<bool> SignInAsync(SignIn signIn, AuthProvider authProvider, CancellationToken cancellationToken = default)
    {
        var payload = new SignInDetails
        {
            UsernameOrEmail = signIn.UsernameOrEmail,
            Password = signIn.Password,
            RememberMe = signIn.RememberMe,
            AuthProvider = authProvider
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

    public async ValueTask<User> SignUpAsync(SignUp signUp, CancellationToken cancellationToken = default)
    {
        var payload = new SignUpDetails
        {
            FirstName = signUp.FirstName,
            LastName = signUp.LastName,
            UserName = signUp.UserName ?? signUp.EmailAddress.Split('@')[0],
            AuthProvider = AuthProvider._1,
            EmailAddress = signUp.EmailAddress,
            Password = signUp.Password,
            AutoGeneratePassword = signUp.Password is null,
            Age = signUp.Age,
        };

        await _apiClient.SignUpAsync(payload, cancellationToken);

        return new User
        {
            FirstName = payload.FirstName,
            LastName = payload.LastName,
            UserName = payload.UserName,
            Email = payload.EmailAddress
        };
    }
}
#endregion