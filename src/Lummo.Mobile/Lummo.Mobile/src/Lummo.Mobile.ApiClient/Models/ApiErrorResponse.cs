using Newtonsoft.Json;

namespace Lummo.Mobile.ApiClient.Models;

public class ApiErrorResponse
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("errorCode")]
    public string ErrorCode { get; set; } = default!;

    [JsonProperty("message")]
    public string Message { get; set; } = default!;

    [JsonProperty("errors")]
    public IEnumerable<string>? Errors { get; set; }
}
