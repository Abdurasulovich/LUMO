namespace Lummo.Mobile.ApiClient.Models;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.6.3.0 (NJsonSchema v11.5.2.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class SignUpDetails
{
    [Newtonsoft.Json.JsonProperty("firstName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string FirstName { get; set; }

    [Newtonsoft.Json.JsonProperty("lastName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string LastName { get; set; }

    [Newtonsoft.Json.JsonProperty("userName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string UserName { get; set; }

    [Newtonsoft.Json.JsonProperty("age", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int Age { get; set; }

    [Newtonsoft.Json.JsonProperty("emailAddress", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string EmailAddress { get; set; }

    [Newtonsoft.Json.JsonProperty("password", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Password { get; set; }

    [Newtonsoft.Json.JsonProperty("autoGeneratePassword", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public bool AutoGeneratePassword { get; set; }

    [Newtonsoft.Json.JsonProperty("authProvider", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public AuthProvider AuthProvider { get; set; }

    [Newtonsoft.Json.JsonProperty("googleIdToken", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string GoogleIdToken { get; set; }
}