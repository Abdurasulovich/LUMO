using Lummo.Mobile.Helpers;
using Lummo.Mobile.Services;

namespace Lummo.Mobile.Views.Pages;

public partial class LoginPage : ContentPage
{
    private bool _isToggling = false;
    private bool _isSigningIn = false;

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

    private async void GoogleSignIn_Handler(object sender, TappedEventArgs e)
    {
        if (_isSigningIn) return;
        _isSigningIn = true;

        try
        {
#if ANDROID
            var authService = new AuthService();
            var result = await authService.SignInWithGoogleAsync();

            if (result.IsSuccess)
            {
                await DisplayAlert(
                    "Muvaffaqiyatli!",
                    $"Xush kelibsiz, {result.DisplayName}!\nEmail: {result.Email}",
                    "OK");

                // TODO: Asosiy sahifaga o'tish
                // await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await DisplayAlert("Xatolik", result.ErrorMessage ?? "Google bilan kirishda xatolik", "OK");
            }
#else
            await DisplayAlert("Xatolik", "Google Sign-In faqat Android da ishlaydi", "OK");
#endif
        }
        catch (Exception ex)
        {
            await DisplayAlert("Xatolik", ex.Message, "OK");
        }
        finally
        {
            _isSigningIn = false;
        }
    }
}