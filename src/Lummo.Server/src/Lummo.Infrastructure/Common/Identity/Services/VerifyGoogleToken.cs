using Google.Apis.Auth;

namespace Lummo.Infrastructure.Common.Identity.Services
{
    public static class VerifyGoogleTokenService
    {
        public static async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[]
                {
                    "608206764181-f1bj129v2u0004uc3e7jhmrjobcpbis2.apps.googleusercontent.com"
                }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return payload;
        }
    }
}
