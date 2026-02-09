namespace Lummo.Mobile.Services.Models;

public class GoogleAuthResult
{
    public bool IsSuccess { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
