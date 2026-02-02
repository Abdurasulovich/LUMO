namespace Lummo.Mobile.Views.Frames;

public partial class VerificationInput : ContentView
{
    public event EventHandler<TextChangedEventArgs>? TextChanged;

    public string Text
    {
        get => CodeEntry.Text ?? string.Empty;
        set => CodeEntry.Text = value;
    }

    public VerificationInput()
    {
        InitializeComponent();
    }

    public void SetFocus()
    {
        CodeEntry.Focus();
    }

    public void ClearFocus()
    {
        CodeEntry.Unfocus();
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        TextChanged?.Invoke(this, e);
    }
}
