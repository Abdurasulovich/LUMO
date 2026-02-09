namespace Lummo.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class IdentityTokenDto
{

    [Newtonsoft.Json.JsonProperty("accessToken", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string AccessToken { get; set; }

    [Newtonsoft.Json.JsonProperty("refreshToken", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string RefreshToken { get; set; }

    [Newtonsoft.Json.JsonProperty("accessTokenExpiryTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.DateTimeOffset AccessTokenExpiryTime { get; set; }

    [Newtonsoft.Json.JsonProperty("refreshTokenExpiryTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.DateTimeOffset RefreshTokenExpiryTime { get; set; }

}