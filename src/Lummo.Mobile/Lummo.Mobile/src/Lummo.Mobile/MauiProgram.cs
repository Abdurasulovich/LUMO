#if ANDROID
using Lummo.Mobile.Platforms.Android.Services;
#endif
#if IOS
using Lummo.Mobile.Platforms.iOS.Services;
#endif
using CommunityToolkit.Maui;
using Lummo.Mobile.ApiClient;
using Lummo.Mobile.ApiClient.Interfaces;
using Lummo.Mobile.Services;
using Lummo.Mobile.Services.Identity.Interfaces;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Xe.AcrylicView;
using Lummo.Mobile.ViewModels;
using Lummo.Mobile.Views.Pages;
using Lummo.Mobile.Services.Identity.Implements;
using Lummo.Mobile.Services.Interfaces;

namespace Lummo.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseAcrylicView()
                .UseSkiaSharp()
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Black.ttf", "PoppinsBlack");
                    fonts.AddFont("Poppins-BlackItalic.ttf", "PoppinsBlackItalic");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                    fonts.AddFont("Poppins-BoldItalic.ttf", "PoppinsBoldItalic");
                    fonts.AddFont("Poppins-ExtraBold.ttf", "PoppinsExtraBold");
                    fonts.AddFont("Poppins-ExtraBoldItalic.ttf", "PoppinsExtraBoldItalic");
                    fonts.AddFont("Poppins-ExtraLight.ttf", "PoppinsExtraLight");
                    fonts.AddFont("Poppins-ExtraLightItalic.ttf", "PoppinsExtraLightItalic");
                    fonts.AddFont("Poppins-Italic.ttf", "PoppinsItalic");
                    fonts.AddFont("Poppins-Light.ttf", "PoppinsLight");
                    fonts.AddFont("Poppins-LightItalic.ttf", "PoppinsLightItalic");
                    fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
                    fonts.AddFont("Poppins-MediumItalic.ttf", "PoppinsMediumItalic");
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemiBold");
                    fonts.AddFont("Poppins-SemiBoldItalic.ttf", "PoppinsSemiBoldItalic");
                    fonts.AddFont("Poppins-Thin.ttf", "PoppinsThin");
                    fonts.AddFont("Poppins-ThinItalic.ttf", "PoppinsThinItalic");
                });

            // Services - AVVAL dependencies keyin API client
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddScoped<ITokenResolver, TokenResolver>();
            builder.Services.AddTransient<AuthHandler>(); // Transient bo'lishi kerak

            // HttpClient va API Client konfiguratsiyasi
            builder.Services.AddHttpClient("LummoApi", client =>
            {
                client.BaseAddress = new Uri("http://192.168.0.103:5188/");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<AuthHandler>();

            // LummoApiClient factory bilan registratsiya - yuqoridagi named HttpClient ishlatiladi
            builder.Services.AddScoped<ILummoApiClient>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("LummoApi");
                return new LummoApiClient("http://192.168.0.103:5188/", httpClient);
            });

            // Boshqa servislar
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ILoadingService, LoadingService>();

            // ViewModels va Pages
            builder.Services.AddScoped<RegisterPageViewModel>();
            builder.Services.AddScoped<VerificationPageViewModel>();
            builder.Services.AddScoped<LoginPageViewModel>();
            builder.Services.AddScoped<ForgotPasswordPageViewModel>();
            builder.Services.AddScoped<ResetPasswordPageViewModel>();


            builder.Services.AddScoped<RegisterPage>();
            builder.Services.AddScoped<VerificationPage>();
            builder.Services.AddScoped<ForgotPasswordPage>();
            builder.Services.AddScoped<ResetPasswordPage>();

#if ANDROID
            builder.Services.AddSingleton<IAuthService, AuthService>();
#endif
#if IOS
            builder.Services.AddSingleton<IAuthService, AuthService>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}