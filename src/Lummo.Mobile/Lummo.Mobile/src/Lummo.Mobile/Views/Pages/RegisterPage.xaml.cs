using Lummo.Mobile.Views.Popups;
using Mopups.Services;

namespace Lummo.Mobile.Views.Pages;

public partial class RegisterPage : ContentPage
{
    private bool _isEyeToggling = false;
    private bool _isReEyeToggling = false;
    public RegisterPage()
	{
        InitializeComponent();
	}

    private async void ReEnterEyeButtonClicked(object sender, EventArgs e)
    {
        if (_isReEyeToggling) return;

        _isReEyeToggling = true;

        // Animatsiya qo'shish
        await ReEnterEyeButton.ScaleTo(0.8, 50);
        await ReEnterEyeButton.ScaleTo(1, 50);

        ReEnterPassworInput.IsPassword = !ReEnterPassworInput.IsPassword;
        ReEnterEyeButton.Source = ReEnterPassworInput.IsPassword ? "eye_crossed" : "eye";

        _isReEyeToggling = false;
    }

    private async void EyeButtonClicked(object sender, EventArgs e)
    {
        if (_isEyeToggling) return;

        _isEyeToggling = true;

        // Animatsiya qo'shish
        await EyeButton.ScaleTo(0.8, 50);
        await EyeButton.ScaleTo(1, 50);

        PasswordInput.IsPassword = !PasswordInput.IsPassword;
        EyeButton.Source = PasswordInput.IsPassword ? "eye_crossed" : "eye";

        _isEyeToggling = false;
    }

    private void PrivacyPolicyAgreement_Handler(object sender, TappedEventArgs e)
    {
        Checkbox.IsChecked = !Checkbox.IsChecked;
    }

    private async void GoToLoginPage_Handler(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..", true);
    }

    private async void OpenLoading_Handler(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PushAsync(new LoadingPopup());
    }
}