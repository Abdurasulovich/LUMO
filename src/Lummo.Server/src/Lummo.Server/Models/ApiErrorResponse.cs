namespace Lummo.Server.Models;

public class ApiErrorResponse
{
    public int StatusCode { get; set; }

    public string ErrorCode { get; set; } = default!;

    public string Message { get; set; } = default!;

    public IEnumerable<string>? Errors { get; set; }
}
