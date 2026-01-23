using FluentValidation;
using Lummo.Application.Common.Settings;
using Lummo.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Lummo.Infrastructure.Common.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator(IOptions<ValidationSettings> validationSettings)
    {
        var validationSettingsValue = validationSettings.Value;


        // Email validation
        RuleFor(user => user.EmailAddress)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(64);

        if (!string.IsNullOrWhiteSpace(validationSettingsValue.EmailRegexPattern))
        {
            RuleFor(user => user.EmailAddress)
                .Matches(validationSettingsValue.EmailRegexPattern)
                .WithMessage("Email address format is invalid.");
        }

        // Username validation
        RuleFor(user => user.UserName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64);

        if (!string.IsNullOrWhiteSpace(validationSettingsValue.UsernameRegexPattern))
        {
            RuleFor(user => user.UserName)
                .Matches(validationSettingsValue.UsernameRegexPattern)
                .WithMessage("Username format is invalid.");
        }

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

        RuleFor(user => user.UserCredentials.PasswordHash)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(64);

    }
}
