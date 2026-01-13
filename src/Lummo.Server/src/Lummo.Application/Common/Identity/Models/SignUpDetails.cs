namespace Lummo.Application.Common.Identity.Models;

public class SignUpDetails
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string EmailAddress { get; set; } = default!;

    public string? Password { get; set; }

    public DateTime BirthDate { get; set; }

    public bool AutoGeneratePassword { get; set; }
}
