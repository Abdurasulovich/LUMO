namespace Lummo.Application.Common.Identity.Models;

public class EmailVerificationDetails
{
    public string EmailAddress { get; set; }

    public string VerificationCode { get; set; }
}
