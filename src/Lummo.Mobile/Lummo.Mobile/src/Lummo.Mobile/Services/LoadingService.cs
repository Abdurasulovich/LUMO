using Lummo.Mobile.Services.Interfaces;
using Lummo.Mobile.Views.Popups;
using Mopups.Interfaces;
using Mopups.Services;

namespace Lummo.Mobile.Services;

public class LoadingService : ILoadingService
{
    private readonly IPopupNavigation navigation;

    public LoadingService()
    {
        navigation = MopupService.Instance;
    }

    public async Task<IDisposable> Show()
    {
        await navigation.PushAsync(new LoadingPopup(), true);
        return new LoadingDisposable(navigation);
    }

    private class LoadingDisposable : IDisposable
    {
        private readonly IPopupNavigation _navigation;
        private bool _disposed;

        public LoadingDisposable(IPopupNavigation navigation)
        {
            _navigation = navigation;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            // Use MainThread to avoid blocking and deadlock issues
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await _navigation.PopAsync();
                }
                catch
                {
                    // Swallow exceptions to prevent app crashes
                }
            });
        }
    }
}
