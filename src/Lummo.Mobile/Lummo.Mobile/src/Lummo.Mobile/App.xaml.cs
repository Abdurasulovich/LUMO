using Microsoft.Maui.Platform;
using System.Globalization;
namespace Lummo.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper
                .AppendToMapping(nameof(Entry),
                (handler, view) =>
                {
                    handler.PlatformView.BackgroundTintList =
                        Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());

                    handler.PlatformView.TextCursorDrawable?.SetTint(Android.Graphics.Color.ParseColor("#135bec"));
                });
#elif IOS
            Microsoft.Maui.Handlers.EntryHandler.Mapper
            .AppendToMapping(nameof(Entry),
            (handler, view) =>
            {
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
                handler.PlatformView.TintColor = UIKit.UIColor.FromRGB(19, 91, 236); // #135bec
            });
#endif
        }
    }
}