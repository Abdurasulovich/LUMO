using Lummo.Mobile.Helpers;

namespace Lummo.Mobile.Views.Pages;

public partial class LoginPage : ContentPage
{
    private bool _isToggling = false;
    public LoginPage()
    {
        InitializeComponent();
    }

    public async Task InitializeAsync()
    {
        try
        {
            ThemeService.Instance.NotifyThemeChanged();
        }
        catch (Exception ex)
        {
            return;
        }
    }
    private async void EyeButtonClicked(object sender, EventArgs e)
    {
        if (_isToggling) return;

        _isToggling = true;

        // Animatsiya qo'shish
        await EyeButton.ScaleTo(0.8, 50);
        await EyeButton.ScaleTo(1, 50);

        PasswordInput.IsPassword = !PasswordInput.IsPassword;
        EyeButton.Source = PasswordInput.IsPassword ? "eye_crossed" : "eye";

        _isToggling = false;
    }

    private async void GoToDefaultPage_Handler(object sender, TappedEventArgs e)
    {
    }

    private async void GoToRegisterPage_Handler(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}