using Lummo.Mobile.Services.Models;

namespace Lummo.Mobile.Services.Identity.Interfaces;

public interface ITokenResolver
{
    Task<TokenInfo> GetTokenAsync();
    Task SetTokenAsync(TokenInfo token);
    Task ClearTokenAsync();
}