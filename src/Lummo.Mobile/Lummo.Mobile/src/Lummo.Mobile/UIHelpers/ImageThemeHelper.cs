using Microsoft.Maui.Platform;

namespace Lummo.Mobile.UIHelpers;

public static class ImageThemeHelper
{
    public static readonly BindableProperty UseThemeColorProperty =
        BindableProperty.CreateAttached(
            "UseThemeColor",
            typeof(bool),
            typeof(ImageThemeHelper),
            false,
            propertyChanged: OnUseThemeColorChanged);

    public static bool GetUseThemeColor(BindableObject view)
        => (bool)view.GetValue(UseThemeColorProperty);

    public static void SetUseThemeColor(BindableObject view, bool value)
        => view.SetValue(UseThemeColorProperty, value);

    // Color for Light mode
    public static readonly BindableProperty LightColorProperty =
        BindableProperty.CreateAttached(
            "LightColor",
            typeof(string),
            typeof(ImageThemeHelper),
            "#97a5ba"); // Default .NET MAUI color

    public static string GetLightColor(BindableObject view)
        => (string)view.GetValue(LightColorProperty);

    public static void SetLightColor(BindableObject view, string value)
        => view.SetValue(LightColorProperty, value);

    // Color for Dark mode
    public static readonly BindableProperty DarkColorProperty =
        BindableProperty.CreateAttached(
            "DarkColor",
            typeof(string),
            typeof(ImageThemeHelper),
            "#FFFFFF"); // Default white color

    public static string GetDarkColor(BindableObject view)
        => (string)view.GetValue(DarkColorProperty);

    public static void SetDarkColor(BindableObject view, string value)
        => view.SetValue(DarkColorProperty, value);

    private static readonly Dictionary<Image, EventHandler> _themeHandlers = new();
    private static readonly Dictionary<Image, EventHandler<AppThemeChangedEventArgs>> _requestedThemeHandlers = new();
    private static readonly Dictionary<Image, EventHandler> _handlerChangedHandlers = new();

    private static void OnUseThemeColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Image image) return;

        CleanupHandlers(image);

        if ((bool)newValue)
        {
            EventHandler handlerChanged = null;
            handlerChanged = (s, e) =>
            {
                if (image.Handler != null)
                {
                    UpdateImageColor(image);
                    image.HandlerChanged -= handlerChanged;
                    _handlerChangedHandlers.Remove(image);
                }
            };

            image.HandlerChanged += handlerChanged;
            _handlerChangedHandlers[image] = handlerChanged;

            if (image.Handler != null)
            {
                UpdateImageColor(image);
                image.HandlerChanged -= handlerChanged;
                _handlerChangedHandlers.Remove(image);
            }

            EventHandler themeHandler = (s, e) =>
            {
                if (image.Handler != null)
                {
                    UpdateImageColor(image);
                }
            };

            EventHandler<AppThemeChangedEventArgs> requestedHandler = (s, e) =>
            {
                if (image.Handler != null)
                {
                    UpdateImageColor(image);
                }
            };

            ThemeHelper.Instance.ThemeChanged += themeHandler;
            Application.Current.RequestedThemeChanged += requestedHandler;

            _themeHandlers[image] = themeHandler;
            _requestedThemeHandlers[image] = requestedHandler;

            image.Unloaded += OnImageUnloaded;
        }
    }

    private static void OnImageUnloaded(object sender, EventArgs e)
    {
        if (sender is Image image)
        {
            CleanupHandlers(image);
            image.Unloaded -= OnImageUnloaded;
        }
    }

    private static void CleanupHandlers(Image image)
    {
        if (_themeHandlers.TryGetValue(image, out var themeHandler))
        {
            ThemeHelper.Instance.ThemeChanged -= themeHandler;
            _themeHandlers.Remove(image);
        }

        if (_requestedThemeHandlers.TryGetValue(image, out var requestedHandler))
        {
            Application.Current.RequestedThemeChanged -= requestedHandler;
            _requestedThemeHandlers.Remove(image);
        }

        if (_handlerChangedHandlers.TryGetValue(image, out var handlerChanged))
        {
            image.HandlerChanged -= handlerChanged;
            _handlerChangedHandlers.Remove(image);
        }
    }

    private static void UpdateImageColor(Image image)
    {
        try
        {
            var theme = Application.Current.UserAppTheme;

            if (theme == AppTheme.Unspecified)
            {
                theme = Application.Current.RequestedTheme;
            }

            // Get the colors - if not given uses default values
            var lightColorHex = GetLightColor(image);
            var darkColorHex = GetDarkColor(image);

            var color = theme == AppTheme.Dark
                ? Color.FromArgb(darkColorHex)
                : Color.FromArgb(lightColorHex);
#if ANDROID
            if (image.Handler?.PlatformView is Android.Widget.ImageView androidImage)
            {
                var colorFilter = new Android.Graphics.PorterDuffColorFilter(
                    color.ToPlatform(),
                    Android.Graphics.PorterDuff.Mode.SrcIn);
                androidImage.SetColorFilter(colorFilter);
            }
#elif IOS
            if (image.Handler?.PlatformView is UIKit.UIImageView iosImage)
            { 
                iosImage.Image = iosImage.Image?.ImageWithRenderingMode(
                    UIKit.UIImageRenderingMode.AlwaysTemplate);
                iosImage.TintColor = color.ToPlatform();
            }
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Image color given error: {ex.Message}");
        }
    }
}