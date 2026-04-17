namespace Lummo.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class ResetPasswordDetails
{

    [Newtonsoft.Json.JsonProperty("userId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Guid UserId { get; set; }

    [Newtonsoft.Json.JsonProperty("newPassword", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string NewPassword { get; set; }

}
