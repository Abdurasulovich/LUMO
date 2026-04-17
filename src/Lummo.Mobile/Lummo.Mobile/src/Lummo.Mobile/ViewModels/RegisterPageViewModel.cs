using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lummo.Mobile.ApiClient.Exceptions;
using Lummo.Mobile.Services.Enums;
using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Services.Interfaces;
using Lummo.Mobile.Services.Models;
using Lummo.Mobile.Views.Pages;
using Mopups.Services;

namespace Lummo.Mobile.ViewModels;

public partial class RegisterPageViewModel : ObservableObject
{
    #region Services
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly ILoadingService _loadingService;
    #endregion

    #region Properies and fields
    public bool isRegisterUserButtonClicked = false;
    #endregion

    [ObservableProperty]
    private string _firstName;
    [ObservableProperty]
    private string _lastName;
    [ObservableProperty]
    private string _userName;
    [ObservableProperty]
    private string _emailAddress;
    [ObservableProperty]
    private string _password;
    [ObservableProperty]
    private string _repeatPassword;
    [ObservableProperty]
    private bool _isAgreePrivacyAndTermsOfUse = false;

    [ObservableProperty]
    private int _isFirstNameNotInput = 0;
    [ObservableProperty]
    private int _isLastNameNotInput = 0;
    [ObservableProperty]
    private int _isUserNameNotInput = 0;
    [ObservableProperty]
    private int _isEmailAdddressNotInputOrNotCorrect = 0;
    [ObservableProperty]
    private int _isPasswordNotInputOrNotMatch = 0;
    public RegisterPageViewModel(IUserService userService,
    IAuthService authService,
    ILoadingService loadingService)
    {
        _userService = userService;
        _authService = authService;
        _loadingService = loadingService;

    }

    #region Commands
    [RelayCommand]
    private async Task ClickGoogle()
    {
        try
        {
            using (await _loadingService.Show())
            {
#if ANDROID
                var result = await _authService.SignUpWithGoogleServiceAsync(default);
                if (result)
                {
                    // TODO: Asosiy sahifaga o'tish
                    await Shell.Current.GoToAsync("//DashboardPage", false);
                }
#endif
            }
        }
        catch (ApiException apiEx)
        {
            await Shell.Current.DisplayAlert("Xatolik", apiEx.GetUserMessage(), "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Xatolik", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task RegisterUser()
    {
        isRegisterUserButtonClicked = true;
        if (string.IsNullOrEmpty(FirstName))
            IsFirstNameNotInput = 1;
        if (string.IsNullOrEmpty(LastName))
            IsLastNameNotInput = 1;
        if (string.IsNullOrEmpty(UserName))
            IsUserNameNotInput = 1;
        if (string.IsNullOrEmpty(EmailAddress))
            IsEmailAdddressNotInputOrNotCorrect = 1;
        if (string.IsNullOrEmpty(Password))
        {
            IsPasswordNotInputOrNotMatch = 1;
            return;
        }
        if (string.IsNullOrEmpty(RepeatPassword))
        {
            IsPasswordNotInputOrNotMatch = 1;
            return;
        }
        if (!string.Equals(Password, RepeatPassword))
        {
            IsPasswordNotInputOrNotMatch = 1;
            return;
        }

        try
        {
            using (await _loadingService.Show())
            {

#if ANDROID
                var signUpDetails = new SignUp
                {
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    EmailAddress = this.EmailAddress,
                    Password = this.Password,
                    UserName = this.UserName,
                    AutoGeneratePassword = string.IsNullOrEmpty(this.Password),
                };

                var signUpResult = await _authService.SignUpAsync(signUpDetails);
                if (signUpResult)
                {
                    var navigationParameter = new Dictionary<string, object>
                {
                    { "EmailAddress", this.EmailAddress},
                    { "FlowType", VerificationFlow.Register }
                };
                    // TODO: Go to verification page
                    await Shell.Current.GoToAsync(nameof(VerificationPage), navigationParameter);
                }
#endif
            }
        }
        catch (ApiException apiEx)
        {
            await Shell.Current.DisplayAlert("Xatolik", apiEx.GetUserMessage(), "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Xatolik", ex.Message, "OK");
        }
    }


    #region Helper functions
    public void FirstNameInput()
    {
        if (isRegisterUserButtonClicked)
        {
            if (FirstName.Length > 2)
                IsFirstNameNotInput = 0;
        }
    }

    public void LastNameInput()
    {
        if (isRegisterUserButtonClicked && LastName.Length > 2)
            IsLastNameNotInput = 0;
    }

    public void UserNameInput()
    {
        if (isRegisterUserButtonClicked && UserName.Length > 2)
            IsUserNameNotInput = 0;
    }
    public void EmailAddressInput()
    {
        if (isRegisterUserButtonClicked)
        {
            if (EmailAddress.Contains("@icloud.com")
                || EmailAddress.Contains("@gmail.com")
                || EmailAddress.Contains("@email.com"))
            {
                IsEmailAdddressNotInputOrNotCorrect = 0;
            }
        }
    }
    public void PasswordInput()
    {
        if (isRegisterUserButtonClicked && Password.Length >= 8)
            IsPasswordNotInputOrNotMatch = 0;
    }
    public void ReEnterPasswordInput()
    {
        if (isRegisterUserButtonClicked && RepeatPassword.Length >= 8
            && Password.Equals(RepeatPassword))
            IsPasswordNotInputOrNotMatch = 0;
    }
    #endregion

    #endregion
}
