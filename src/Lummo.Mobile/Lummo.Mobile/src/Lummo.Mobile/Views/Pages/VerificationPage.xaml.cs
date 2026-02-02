using Lummo.Mobile.Views.Frames;
using Lummo.Mobile.Views.Popups;
using Mopups.Services;

namespace Lummo.Mobile.Views.Pages;

public partial class VerificationPage : ContentPage
{
    private VerificationInput[] _inputs = null!;
    private IDispatcherTimer _timer = null!;
    private int _remainingSeconds = 60;

    public VerificationPage()
    {
        InitializeComponent();
        SetupInputs();
        StartCountdown();
    }

    private void SetupInputs()
    {
        _inputs = [firstInput, secondInput, thirdInput, fourthInput, fifthInput, sixthInput];

        for (int i = 0; i < _inputs.Length; i++)
        {
            int index = i;
            _inputs[i].TextChanged += (sender, e) => OnInputTextChanged(index, e);
        }
    }

    private void StartCountdown()
    {
        _remainingSeconds = 60;
        UpdateTimerDisplay();

        if (_timer == null)
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
        }

        _timer.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _remainingSeconds--;
        UpdateTimerDisplay();

        if (_remainingSeconds <= 0)
        {
            _timer.Stop();
            ShowResendButton();
        }
    }

    private void ShowResendButton()
    {
        TimerSection.IsVisible = false;
        ResendButton.IsVisible = true;
    }

    private void ShowTimer()
    {
        TimerSection.IsVisible = true;
        ResendButton.IsVisible = false;
    }

    private async void ResendCode_Handler(object sender, TappedEventArgs e)
    {
        // TODO: Backend ga yangi kod so'rovi yuborish
        // await _verificationService.ResendCodeAsync();

        ShowTimer();
        StartCountdown();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = _remainingSeconds / 60;
        int seconds = _remainingSeconds % 60;

        MinutesSpan.Text = minutes.ToString("D2");
        SecondsSpan.Text = seconds.ToString("D2");
    }

    private void OnInputTextChanged(int index, TextChangedEventArgs e)
    {
        // Raqam kiritilganda keyingi inputga o'tish
        if (!string.IsNullOrEmpty(e.NewTextValue) && index < _inputs.Length - 1)
        {
            _inputs[index + 1].SetFocus();
        }
        // O'chirilganda oldingi inputga qaytish
        else if (string.IsNullOrEmpty(e.NewTextValue) && index > 0)
        {
            _inputs[index - 1].SetFocus();
        }

        // Agar oxirgi input to'ldirilsa, klaviaturani yopish
        if (index == _inputs.Length - 1 && !string.IsNullOrEmpty(e.NewTextValue))
        {
            _inputs[index].ClearFocus();
        }
    }

    public string GetVerificationCode()
    {
        return string.Concat(_inputs.Select(i => i.Text));
    }

    private async void GoToRegisterPage_Handler(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..", true);
    }

    private async void VerifyCode_Handler(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PushAsync(new LoadingPopup());
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer?.Stop();
    }
}
