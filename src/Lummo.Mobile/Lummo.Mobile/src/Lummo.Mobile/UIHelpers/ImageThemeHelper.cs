using Microsoft.Maui.Platform;
using System.Runtime.CompilerServices;

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

    public static readonly BindableProperty LightColorProperty =
        BindableProperty.CreateAttached("LightColor", typeof(string),
            typeof(ImageThemeHelper), "#97a5ba");
    public static string GetLightColor(BindableObject view)
        => (string)view.GetValue(LightColorProperty);
    public static void SetLightColor(BindableObject view, string value)
        => view.SetValue(LightColorProperty, value);

    public static readonly BindableProperty DarkColorProperty =
        BindableProperty.CreateAttached("DarkColor", typeof(string),
            typeof(ImageThemeHelper), "#FFFFFF");
    public static string GetDarkColor(BindableObject view)
        => (string)view.GetValue(DarkColorProperty);
    public static void SetDarkColor(BindableObject view, string value)
        => view.SetValue(DarkColorProperty, value);

    // ── State ─────────────────────────────────────────────────────────────────
    private sealed class ImageState
    {
        public EventHandler? ThemeHandler { get; set; }
        public EventHandler<AppThemeChangedEventArgs>? RequestedHandler { get; set; }
        public EventHandler? HandlerChangedHandler { get; set; }
        public EventHandler? LoadedHandler { get; set; }
        public bool IsSubscribed { get; set; }
    }

    private static readonly ConditionalWeakTable<Image, ImageState> _states = new();

    // ── Entry point ───────────────────────────────────────────────────────────
    private static void OnUseThemeColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Image image) return;
        Unsubscribe(image);
        if ((bool)newValue) Subscribe(image);
    }

    private static void Subscribe(Image image)
    {
        var state = _states.GetOrCreateValue(image);
        if (state.IsSubscribed) return;

        // 1) Handler tayyor bo'lganda rang ber (birinchi yuklash)
        EventHandler? handlerChanged = null;
        handlerChanged = (s, e) =>
        {
            if (image.Handler is null) return;
            UpdateImageColor(image);
            image.HandlerChanged -= state.HandlerChangedHandler;
            state.HandlerChangedHandler = null;
        };
        state.HandlerChangedHandler = handlerChanged;
        image.HandlerChanged += handlerChanged;

        if (image.Handler is not null)
        {
            UpdateImageColor(image);
            image.HandlerChanged -= handlerChanged;
            state.HandlerChangedHandler = null;
        }

        // 2) Navigation qaytganda — Loaded qayta chaqiriladi
        //    HandlerChanging ishonchsiz, shuning uchun Loaded ishlatamiz
        EventHandler? loadedHandler = null;
        loadedHandler = (s, e) =>
        {
            // Handler hali attach bo'lmagan bo'lishi mumkin, post qilamiz
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (image.Handler is not null)
                    UpdateImageColor(image);
            });
        };
        state.LoadedHandler = loadedHandler;
        image.Loaded += loadedHandler;

        // 3) Theme o'zgarganda rang yangilansin
        EventHandler themeHandler = (s, e) =>
        {
            if (image.Handler is null) return;
            UpdateImageColor(image);
        };
        EventHandler<AppThemeChangedEventArgs> requestedHandler = (s, e) =>
        {
            if (image.Handler is null) return;
            UpdateImageColor(image);
        };

        state.ThemeHandler = themeHandler;
        state.RequestedHandler = requestedHandler;
        state.IsSubscribed = true;

        ThemeHelper.Instance.ThemeChanged += themeHandler;
        Application.Current!.RequestedThemeChanged += requestedHandler;

        image.Unloaded += OnImageUnloaded;
    }

    private static void Unsubscribe(Image image)
    {
        if (!_states.TryGetValue(image, out var state)) return;

        if (state.ThemeHandler is not null)
        {
            ThemeHelper.Instance.ThemeChanged -= state.ThemeHandler;
            state.ThemeHandler = null;
        }

        if (state.RequestedHandler is not null)
        {
            if (Application.Current is not null)
                Application.Current.RequestedThemeChanged -= state.RequestedHandler;
            state.RequestedHandler = null;
        }

        if (state.HandlerChangedHandler is not null)
        {
            image.HandlerChanged -= state.HandlerChangedHandler;
            state.HandlerChangedHandler = null;
        }

        if (state.LoadedHandler is not null)
        {
            image.Loaded -= state.LoadedHandler;
            state.LoadedHandler = null;
        }

        image.Unloaded -= OnImageUnloaded;
        state.IsSubscribed = false;
        _states.Remove(image);
    }

    private static void OnImageUnloaded(object? sender, EventArgs e)
    {
        // Unsubscribe emas — faqat Unloaded event'ni saqlab qo'yamiz
        // Chunki Loaded qayta ishlashi kerak (navigation qaytganda)
        // Faqat native handle tozalanadi, subscription qoladi
    }

    private static void UpdateImageColor(Image image)
    {
        try
        {
            var theme = Application.Current?.UserAppTheme ?? AppTheme.Unspecified;
            if (theme == AppTheme.Unspecified)
                theme = Application.Current?.RequestedTheme ?? AppTheme.Light;

            var color = theme == AppTheme.Dark
                ? Color.FromArgb(GetDarkColor(image))
                : Color.FromArgb(GetLightColor(image));

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
                iosImage.Image = iosImage.Image?
                    .ImageWithRenderingMode(UIKit.UIImageRenderingMode.AlwaysTemplate);
                iosImage.TintColor = color.ToPlatform();
            }
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ImageThemeHelper] {ex.Message}");
        }
    }
}