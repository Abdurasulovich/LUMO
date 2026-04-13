using System.Security.Principal;

namespace Lummo.Application.Common.Identity.Models;

public class ResendVerificationCodeRequest
{
    public string EmailAddress { get; set; } = string.Empty;
}
