namespace Lummo.Mobile.Services.Models;

public class SignIn
{
    public string UsernameOrEmail { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool RememberMe { get; set; }
}