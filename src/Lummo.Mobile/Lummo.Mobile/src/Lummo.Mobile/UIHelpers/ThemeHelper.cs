namespace Lummo.Mobile.UIHelpers;

public class ThemeHelper
{
    private static ThemeHelper _instance;
    public static ThemeHelper Instance => _instance ?? (_instance = new ThemeHelper());

    public event EventHandler ThemeChanged;
    public void NotifyThemeChanged()
    {
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }
}