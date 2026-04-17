using CommunityToolkit.Mvvm.ComponentModel;
using Lummo.Mobile.ApiClient.Models;
using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Services.Interfaces;
using Lummo.Mobile.Views.Pages;

namespace Lummo.Mobile.ViewModels;


public partial class VerificationPageViewModel : ObservableObject, IQueryAttributable
{
    private string _emailAddress;
    private string _flowType;
    #region Services
    private readonly IAuthService _authService;
    private readonly ILoadingService _loadingService;
    #endregion

    #region Properies and fields

    #endregion

    public VerificationPageViewModel(IAuthService? authService,
        ILoadingService? loadingService)
    {
        _authService = authService;
        _loadingService = loadingService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("EmailAddress"))
            _emailAddress = query["EmailAddress"]?.ToString() ?? string.Empty;

        if (query.ContainsKey("FlowType"))
            _flowType = query["FlowType"]?.ToString() ?? string.Empty;
    }

    #region Commands
    public async Task ResendCode()
    {
        try
        {
            await _authService.ResendVerificationCode(new ResendVerificationCodeRequest { EmailAddress = _emailAddress });
        }
        catch (Exception ex)
        {

        }
    }
    public async Task VerifyCode(string code)
    {
        try
        {
            using (await _loadingService.Show())
            {
                var details = new EmailVerificationDetails
                {
                    EmailAddress = _emailAddress,
                    VerificationCode = code
                };

                if (_flowType == "Register")
                {
                    var isVerified = await _authService.VerifyEmail(details);
                    if(isVerified) 
                        await Shell.Current.GoToAsync("//DashboardPage");
                }
                else if (_flowType == "ForgotPassword")
                {
                    var isVerified = await _authService.ForgotPasswordVerifyEmailAsync(details);
                    var navParams = new Dictionary<string, object>
                {
                    { "EmailAddress", _emailAddress }
                };

                    await Shell.Current.GoToAsync(nameof(ResetPasswordPage), navParams);
                }
            }
        }
        catch (Exception ex)
        {
            // handle error
        }
    }
    #endregion
}
