using FluentValidation;
using Lummo.Application.Common.Identity.Models;
using Lummo.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Lummo.Infrastructure.Validators;

public class SignUpDetailsValidator : AbstractValidator<SignUpDetails>
{
    public SignUpDetailsValidator(IOptions<ValidationSettings> validationSettings, 
        IOptions<PasswordValidationSettings> passwordValidationSettings)
    {
        var validationSettingsValue = validationSettings.Value;
        var passwordValidationSettingsValue = passwordValidationSettings.Value;

        RuleFor(signup => signup.EmailAddress)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(64)
            .Matches(validationSettingsValue.EmailAddressRegexPattern);
        RuleFor(signup => signup.FirstName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64)
            .Matches(validationSettingsValue.NameRegexPattern)
            .WithMessage("First name is not valid.");

        RuleFor(signup => signup.LastName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64)
            .Matches(validationSettingsValue.NameRegexPattern)
            .WithMessage("Last name is not valid.");

        RuleFor(signup => signup.BirthDate).Must(BeAValidAge).WithMessage("Age must greater than 18 and less than 130");

        RuleFor(signup => signup.Password)
            .NotNull()
            .WithMessage("Password is required.")
            .MinimumLength(passwordValidationSettingsValue.MinimumLength)
            .WithMessage($"Password must be at least {passwordValidationSettingsValue.MinimumLength} characters long.")
            .MaximumLength(passwordValidationSettingsValue.MaximumLength)
            .WithMessage($"Password must be at most {passwordValidationSettingsValue.MaximumLength} characters long.")
            .Custom(
            (password, context) =>
            {
                if (passwordValidationSettingsValue.RequireDigit && !password.Any(char.IsDigit))
                    context.AddFailure("Password must contain at least one digit.");
            })
            .Custom(
            (password, context) =>
            {
                if (passwordValidationSettingsValue.RequireUppercase && !password.Any(char.IsUpper))
                    context.AddFailure("Password must contain at least one uppercase letter.");
            })
            .Custom(
                (password, context) =>
                {
                    if (passwordValidationSettingsValue.RequireLowercase && !password.Any(char.IsLower))
                        context.AddFailure("Password must contain at least one lowercase letter.");
                }
            )
            .Custom(
                (password, context) =>
                {
                    if (passwordValidationSettingsValue.RequireNonAlphanumeric && !password.All(char.IsLetterOrDigit))
                        context.AddFailure("Password must contain at least one non-alphanumeric character.");
                }
            )
            .When(signup => !signup.AutoGeneratePassword);
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
