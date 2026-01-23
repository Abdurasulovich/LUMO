namespace Lummo.Application.Common.Identity.Models;

public class SignInDetails
{
    public string UsernameOrEmail { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool RememberMe { get; set; }
}
