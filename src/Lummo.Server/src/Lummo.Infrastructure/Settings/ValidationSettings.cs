namespace Lummo.Infrastructure.Settings;

public class ValidationSettings
{
    public string EmailAddressRegexPattern { get; set; } = default!;

    public string UrlRegexPattern { get; set; } = default!;
}
