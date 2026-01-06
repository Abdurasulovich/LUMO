using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace Lummo.Mobile
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Edge-to-edge rejimi
            WindowCompat.SetDecorFitsSystemWindows(Window, false);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.S) // Android 12+
            {
                Window?.SetStatusBarColor(Android.Graphics.Color.Transparent);
                Window?.SetNavigationBarColor(Android.Graphics.Color.Transparent);

                // Blur effect
                Window?.AddFlags(WindowManagerFlags.BlurBehind);
                if (Window?.Attributes != null)
                {
                    Window.Attributes.BlurBehindRadius = 50;
                }
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.R) // Android 11
            {
                Window?.SetStatusBarColor(Android.Graphics.Color.Transparent);
                Window?.SetNavigationBarColor(Android.Graphics.Color.Transparent);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) // Android 5.0+
            {
                Window?.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window?.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window?.SetStatusBarColor(Android.Graphics.Color.Transparent);
                Window?.SetNavigationBarColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}