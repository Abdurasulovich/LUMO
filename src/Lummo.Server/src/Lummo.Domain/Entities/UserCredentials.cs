namespace Lummo.Domain.Entities;

public class UserCredentials
{
    public UserCredentials(){}

    public UserCredentials(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public string PasswordHash { get; set; } = default!;
}
