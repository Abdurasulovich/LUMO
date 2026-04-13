namespace Lummo.Application.Common.Identity.Models;

public class ResetPasswordDetails
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; }
}
