using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Services.Models;
using Microsoft.Maui.Storage;

namespace Lummo.Mobile.Services.Identity.Implements;

public class TokenResolver : ITokenResolver
{
    private const string AccessTokenKey = "access_token_key";
    private const string RefreshTokenKey = "refresh_token_key";
    private const string AccessTokenExpiresAtKey = "expires_at_key";
    private const string RefreshTokenExpiresAtKey = "expires_at_key";
    public Task ClearTokenAsync()
    {
        try
        {
            SecureStorage.Remove(AccessTokenKey);
            SecureStorage.Remove(RefreshTokenKey);
            SecureStorage.Remove(AccessTokenExpiresAtKey);
            SecureStorage.Remove(RefreshTokenExpiresAtKey);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.CompletedTask;
        }
    }

    public async Task<TokenInfo> GetTokenAsync()
    {
        try
        {
            var accessToken = await SecureStorage.GetAsync(AccessTokenKey);
            var refreshToken = await SecureStorage.GetAsync(RefreshTokenKey);
            var accessExpiresAtString = await SecureStorage.GetAsync(AccessTokenExpiresAtKey);
            var refreshExpiresAtString = await SecureStorage.GetAsync(RefreshTokenExpiresAtKey);

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(accessExpiresAtString)
                || string.IsNullOrEmpty(refreshExpiresAtString) || string.IsNullOrEmpty(refreshToken))
                return null;

            return new TokenInfo
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = DateTimeOffset.Parse(accessExpiresAtString),
                RefreshTokenExpiresAt = DateTimeOffset.Parse(refreshExpiresAtString)
            };
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task SetTokenAsync(TokenInfo token)
    {
        try
        {
            if (token == null)
                return;
            await SecureStorage.SetAsync(AccessTokenKey, token.AccessToken);
            await SecureStorage.SetAsync(RefreshTokenKey, token.RefreshToken);
            await SecureStorage.SetAsync(AccessTokenExpiresAtKey, token.AccessTokenExpiresAt.ToString("0"));
            await SecureStorage.SetAsync(RefreshTokenExpiresAtKey, token.RefreshTokenExpiresAt.ToString("0"));
        }
        catch (Exception ex)
        {
            return;
        }
    }
}
