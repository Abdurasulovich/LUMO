namespace Lummo.Application.Common.Settings;

public class ValidationSettings
{
    public string EmailRegexPattern { get; set; } = default!;
    public string NameRegexPattern { get; set; }= default!;
    public string PasswordRegexPattern { get; set; } = default!;
    public string UrlRegexPattern { get; set; } = default!;
    public string FileNameRegexPattern { get; set; } = default!;
    public string UsernameRegexPattern { get; set; } = default!;
}
