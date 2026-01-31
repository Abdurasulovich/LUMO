namespace Lummo.Mobile.Helpers;

public class ThemeService
{
    private static ThemeService _instance;
    public static ThemeService Instance => _instance ??= new ThemeService();

    public event EventHandler ThemeChanged;

    public void NotifyThemeChanged()
    {
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }
}
