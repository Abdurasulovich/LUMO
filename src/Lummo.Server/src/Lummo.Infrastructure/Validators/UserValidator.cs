using FluentValidation;
using Lummo.Domain.Entities;
using Lummo.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Lummo.Infrastructure.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator(IOptions<ValidationSettings> validationSettings)
    {
        var validationSettingsValue = validationSettings.Value;


        RuleFor(user => user.EmailAddress)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(64)
            .Matches(validationSettingsValue.EmailAddressRegexPattern);

        RuleFor(user => user.FirstName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64)
            .Matches(validationSettingsValue.NameRegexPattern)
            .WithMessage("First name is not valid.");

        RuleFor(user => user.LastName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64)
            .Matches(validationSettingsValue.NameRegexPattern)
            .WithMessage("Last name is not valid.");

        RuleFor(user => user.BirthDate).Must(BeAValidAge).WithMessage("Age must greater than 18 and less than 130.");
    }

    private bool BeAValidAge(DateTime dateTime)
    {
        int currentYear = DateTime.Now.Year;
        int dobYear = dateTime.Year;

        if (dobYear < (currentYear - 17) && dobYear < (currentYear - 130))
            return true;
        return false;
    }
}
