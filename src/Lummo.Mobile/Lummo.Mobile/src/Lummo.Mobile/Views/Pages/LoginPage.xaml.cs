namespace Lummo.Mobile.Views.Pages;

public partial class LoginPage : ContentPage
{
    private bool _isToggling = false;
    public LoginPage()
    {
        InitializeComponent();
    }

    private void InitializeAsync()
    {
    }

    private async void EyeButtonClicked(object sender, EventArgs e)
    {
        if (_isToggling) return; // Agar hali ishlab bo'lmasa, qaytadan ishlatmaslik

        _isToggling = true;

        // Animatsiya qo'shish
        await EyeButton.ScaleTo(0.8, 50);
        await EyeButton.ScaleTo(1, 50);

        PasswordInput.IsPassword = !PasswordInput.IsPassword;
        EyeButton.Source = PasswordInput.IsPassword ? "eye_crossed" : "eye";

        _isToggling = false;
    }
}