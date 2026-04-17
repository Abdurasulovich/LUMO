using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Services.Interfaces;
using Lummo.Mobile.Services.Models;
using Lummo.Mobile.Views.Pages;
#if ANDROID
using static Android.Telephony.Mbms.MbmsErrors;
using IntelliJ.Lang.Annotations;
#endif
namespace Lummo.Mobile.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    private readonly ILoadingService _loadingService;
    private readonly IAuthService _authService;

    public LoginPageViewModel(ILoadingService loadingService, IAuthService authService)
    {
        _loadingService = loadingService;
        _authService = authService;
    }

    #region Properties

    [ObservableProperty] private string _emailAddress = string.Empty;
    [ObservableProperty] private string _password = string.Empty;

    // Subtitle message
    [ObservableProperty] private string _message = "Ready to ace your next exam?";
    [ObservableProperty] private Color _messageColor = Color.FromArgb("#64748b");

    // Inline field-level errors
    [ObservableProperty] private string _emailError = string.Empty;
    [ObservableProperty] private string _passwordError = string.Empty;

    // General (non-field) error — shown below the button
    [ObservableProperty] private string _generalError = string.Empty;

    [ObservableProperty] private bool _isBusy;

    // Computed visibility
    public bool HasEmailError => !string.IsNullOrEmpty(EmailError);
    public bool HasPasswordError => !string.IsNullOrEmpty(PasswordError);
    public bool HasGeneralError => !string.IsNullOrEmpty(GeneralError);

    #endregion

    #region Partial changed hooks — refresh computed bools

    partial void OnEmailErrorChanged(string value) => OnPropertyChanged(nameof(HasEmailError));
    partial void OnPasswordErrorChanged(string value) => OnPropertyChanged(nameof(HasPasswordError));
    partial void OnGeneralErrorChanged(string value) => OnPropertyChanged(nameof(HasGeneralError));

    // Clear errors as user types
    partial void OnEmailAddressChanged(string value) => EmailError = string.Empty;
    partial void OnPasswordChanged(string value) => PasswordError = string.Empty;

    #endregion

    #region Commands

    [RelayCommand]
    private async Task GotoRegisterPage() =>
        await Shell.Current.GoToAsync(nameof(RegisterPage));

    [RelayCommand]
    private async Task ForgotPassword() =>
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage)); // to'g'ri page nomi

    [RelayCommand]
    private async Task ViaEmail()
    {
        ClearAllErrors();

        if (!ValidateEmailAndPassword()) return;

        if (!await CheckInternetAsync()) return;

        try
        {
            IsBusy = true;
            using (await _loadingService.Show())
            {
                var result = await _authService.SignInAsync(new SignIn
                {
                    UsernameOrEmail = EmailAddress.Trim(),
                    Password = Password
                });

                if (result)
                {
                    await Shell.Current.GoToAsync("//DashboardPage");
                }
                else
                {
                    GeneralError = "Sign in failed. Please try again.";
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ViaGoogle()
    {
        ClearAllErrors();

        if (!await CheckInternetAsync()) return;

        try
        {
            IsBusy = true;
            using (await _loadingService.Show())
            {
                var result = await _authService.SignInWithGoogleServiceAsync();

                if (result)
                {
                    await Shell.Current.GoToAsync("//DashboardPage");
                }
                else
                {
                    GeneralError = "Google sign-in failed. Please try again.";
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Private helpers

    private void ClearAllErrors()
    {
        EmailError = string.Empty;
        PasswordError = string.Empty;
        GeneralError = string.Empty;
        SetSubtitle("Ready to ace your next exam?", "#64748b");
    }

    private void SetSubtitle(string text, string hex)
    {
        Message = text;
        MessageColor = Color.FromArgb(hex);
    }

    private bool ValidateEmailAndPassword()
    {
        var valid = true;

        if (string.IsNullOrWhiteSpace(EmailAddress))
        {
            EmailError = "Email or username is required.";
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            PasswordError = "Password is required.";
            valid = false;
        }
        else if (Password.Length < 8)
        {
            PasswordError = "Password must be at least 8 characters.";
            valid = false;
        }

        return valid;
    }

    private async Task<bool> CheckInternetAsync()
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            return true;

        GeneralError = "No internet connection. Please check your network.";
        SetSubtitle("No connection 📡", "#f97316");
        return false;
    }

    /// <summary>
    /// Backend'dan kelgan har xil xatoliklarni user-friendly xabarga aylantiradi.
    /// </summary>
    private void HandleException(Exception ex)
    {
        var msg = ex.Message ?? string.Empty;

        // HTTP status codeni xabardan parse qilamiz (sizning HttpClient wrapper'ingizga qarab o'zgartiring)
        if (msg.Contains("401") || msg.Contains("invalid", StringComparison.OrdinalIgnoreCase)
                                 || msg.Contains("incorrect", StringComparison.OrdinalIgnoreCase))
        {
            SetSubtitle("Incorrect credentials 🔐", "#d00000");
            GeneralError = "Email/username or password is incorrect.";
        }
        else if (msg.Contains("not verified", StringComparison.OrdinalIgnoreCase)
              || msg.Contains("email address is not verified", StringComparison.OrdinalIgnoreCase))
        {
            SetSubtitle("Verify your email 📧", "#f97316");
            GeneralError = "Your email is not verified. Check your inbox and confirm your address.";
        }
        else if (msg.Contains("register", StringComparison.OrdinalIgnoreCase)
              || msg.Contains("No account found", StringComparison.OrdinalIgnoreCase))
        {
            SetSubtitle("Account not found 🔍", "#f97316");
            GeneralError = "No account found with this email. Please register first.";
        }
        else if (msg.Contains("Google", StringComparison.OrdinalIgnoreCase)
              || msg.Contains("sign in with Google", StringComparison.OrdinalIgnoreCase))
        {
            GeneralError = "This account was registered with Google. Please use 'Continue with Google'.";
        }
        else if (msg.Contains("400"))
        {
            GeneralError = "Invalid request. Please check your input and try again.";
        }
        else if (msg.Contains("500") || msg.Contains("server", StringComparison.OrdinalIgnoreCase))
        {
            GeneralError = "Server error. Please try again later.";
        }
        else if (msg.Contains("timeout", StringComparison.OrdinalIgnoreCase)
              || msg.Contains("TaskCanceledException", StringComparison.OrdinalIgnoreCase))
        {
            GeneralError = "Request timed out. Check your internet and try again.";
        }
        else
        {
            GeneralError = "Something went wrong. Please try again.";
        }
    }

    #endregion
}