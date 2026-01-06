using Foundation;
using UIKit;

namespace Lummo.Mobile
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Configure status bar appearance for iOS
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                // iOS 13+ uses scene-based status bar configuration
                // The status bar style is controlled by the StatusBarBehavior
            }
            else
            {
#pragma warning disable CA1422
                UIApplication.SharedApplication.StatusBarHidden = false;
#pragma warning restore CA1422
            }

            return base.FinishedLaunching(application, launchOptions);
        }
    }
}
