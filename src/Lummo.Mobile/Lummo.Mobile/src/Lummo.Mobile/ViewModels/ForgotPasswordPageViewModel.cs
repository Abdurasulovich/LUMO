using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lummo.Mobile.ApiClient.Models;
using Lummo.Mobile.Services.Enums;
using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Views.Pages;

namespace Lummo.Mobile.ViewModels;

public partial class ForgotPasswordPageViewModel : ObservableObject
{
    #region
    private IAuthService _authService;
    #endregion

    #region Properties
    [ObservableProperty]
    private string _email = string.Empty;
    [ObservableProperty]
    private bool _isEmailAdddressNotInputOrNotCorrect = false;
    [ObservableProperty]
    private bool _hasGeneralError = false;
    [ObservableProperty]
    private string _generalError = string.Empty;
    #endregion


    #region Constructor
    public ForgotPasswordPageViewModel(IAuthService authService)
    {
        _authService = authService;
    }
    #endregion
    #region Commands
    [RelayCommand]
    private async Task GotoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
    [RelayCommand]
    private async Task SendCode()
    {
        if (string.IsNullOrEmpty(Email) || !Email.Contains("@"))
        {
            IsEmailAdddressNotInputOrNotCorrect = true;
            HasGeneralError = true;
            GeneralError = "Please input a correct email address.";
            return;
        }
        var apiResult = await _authService.ForgotPasswordVerifyEmailAsync(new ForgotPasswordEmailVerificationDetails { EmailAddress = Email });

        if (apiResult)
        {
            var navigationParameter = new Dictionary<string, object>
                {
                    { "EmailAddress", this.Email },
                    { "FlowType", VerificationFlow.ForgotPassword}
                };
            // TODO: Go to verification page
            await Shell.Current.GoToAsync(nameof(VerificationPage), navigationParameter);
        }
        else
        {
            HasGeneralError = true;
            IsEmailAdddressNotInputOrNotCorrect = true;
            GeneralError = "No user found with this email address. Please check your email is correct and try again.";
        }
    }
    #endregion
}
