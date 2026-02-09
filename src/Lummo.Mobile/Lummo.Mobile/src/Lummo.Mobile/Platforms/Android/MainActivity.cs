using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api.SignIn;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using gr = Android.Graphics;
namespace Lummo.Mobile.Platforms.Android
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private const int GoogleSignInRequestCode = 9001;

        public static event EventHandler<(bool Success, GoogleSignInAccount? Account, string? Error)>? ResultGoogleAuth;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Edge-to-edge rejimi
            WindowCompat.SetDecorFitsSystemWindows(Window, false);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.S) // Android 12+
            {
                Window?.SetStatusBarColor(gr.Color.Transparent);
                Window?.SetNavigationBarColor(gr.Color.Transparent);

                // Blur effect
                Window?.AddFlags(WindowManagerFlags.BlurBehind);
                if (Window?.Attributes != null)
                {
                    Window.Attributes.BlurBehindRadius = 50;
                }
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.R) // Android 11
            {
                Window?.SetStatusBarColor(gr.Color.Transparent);
                Window?.SetNavigationBarColor(gr.Color.Transparent);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) // Android 5.0+
            {
                Window?.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window?.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window?.SetStatusBarColor(gr.Color.Transparent);
                Window?.SetNavigationBarColor(gr.Color.Transparent);
            }

            // Navigation bar ni yashirish (status bar qoladi)
            HideNavigationBar();
        }

        private void HideNavigationBar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R) // Android 11+
            {
                var controller = Window?.InsetsController;
                if (controller != null)
                {
                    controller.Hide(WindowInsets.Type.NavigationBars());
                    controller.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
                }
            }
            else if (Window?.DecorView != null)
            {
#pragma warning disable CA1422
#pragma warning disable CS0618
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                    SystemUiFlags.HideNavigation |
                    SystemUiFlags.ImmersiveSticky
                );
#pragma warning restore CA1422
            }
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
                HideNavigationBar();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == GoogleSignInRequestCode)
            {
                try
                {
                    var task = GoogleSignIn.GetSignedInAccountFromIntent(data);
                    var account = task.Result as GoogleSignInAccount;

                    if (account != null)
                    {
                        ResultGoogleAuth?.Invoke(this, (true, account, null));
                    }
                    else
                    {
                        ResultGoogleAuth?.Invoke(this, (false, null, "Google hisobini olishda xatolik"));
                    }
                }
                catch (Exception ex)
                {
                    ResultGoogleAuth?.Invoke(this, (false, null, ex.Message));
                }
            }
        }
    }
}