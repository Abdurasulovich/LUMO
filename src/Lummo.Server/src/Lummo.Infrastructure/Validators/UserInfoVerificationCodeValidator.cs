using FluentValidation;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Lummo.Infrastructure.Validators;

public class UserInfoVerificationCodeValidator : AbstractValidator<UserInfoVerificationCode>
{
    public UserInfoVerificationCodeValidator(IOptions<VerificationSettings> verificationSettings,
        IOptions<ValidationSettings> validationSettings)
    {
        var verificationSettingsValue = verificationSettings.Value;
        var validationSettingsValue = validationSettings.Value;

        RuleSet(
            EntityEvent.OnCreate.ToString(),
            () =>
            {
                RuleFor(code => code.UserId).NotEqual(Guid.Empty);
                RuleFor(code => code.ExpiryTime)
                .GreaterThan(DateTimeOffset.UtcNow)
                .LessThanOrEqualTo(
                    DateTimeOffset.UtcNow.AddSeconds(verificationSettingsValue.
                    VerificationCodeExpiryTimeInSeconds));

                RuleFor(code => code.IsActive).Equal(true);
                RuleFor(code => code.VerificationLink).NotEmpty().Matches(validationSettingsValue.UrlRegexPattern);
            });
    }
}
