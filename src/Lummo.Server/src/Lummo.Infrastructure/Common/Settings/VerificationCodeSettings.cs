namespace Lummo.Infrastructure.Common.Settings;

public class VerificationCodeSettings
{
    public string VerificationLink { get; set; } = default!;
    public int VerificationCodeExpiryTimeInSeconds { get; set; }
    public int VerificationCodeLength { get; set; }
}
