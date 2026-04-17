using Lummo.Mobile.Helpers;
using Lummo.Mobile.ViewModels;

namespace Lummo.Mobile.Views.Pages;

public partial class LoginPage : ContentPage
{
    private readonly LoginPageViewModel _vm;
    private bool _isToggling = false;

    public LoginPage(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _vm = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ThemeService.Instance.NotifyThemeChanged();
    }

    private async void EyeButtonClicked(object sender, EventArgs e)
    {
        if (_isToggling) return;
        _isToggling = true;

        await EyeButton.ScaleTo(0.8, 50);
        await EyeButton.ScaleTo(1.0, 50);

        PasswordInput.IsPassword = !PasswordInput.IsPassword;
        EyeButton.Source = PasswordInput.IsPassword ? "eye_crossed" : "eye";

        _isToggling = false;
    }
}