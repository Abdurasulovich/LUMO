namespace Lummo.Mobile.Services.Interfaces;

public interface ILoadingService
{
    Task<IDisposable> Show();
}