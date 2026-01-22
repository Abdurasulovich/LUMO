using FluentValidation;
using Lummo.Application.Common.Settings;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Lummo.Infrastructure.Common.Validators;

public class UserCredentialsValidator : AbstractValidator<UserCredentials>
{
    public UserCredentialsValidator(IOptions<ValidationSettings> validationSettings)
    {
        var validationSettingsValue = validationSettings.Value;

        RuleSet(
            EntityEvent.OnCreate.ToString(),
            () =>
            {
                RuleFor(userCrdentials => userCrdentials.PasswordHash)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(64)
                .Matches(validationSettingsValue.PasswordRegexPattern);
            }
        );
    }
}
