using FluentValidation;
using Lummo.Application.Common.Identity.Models;
using Lummo.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace Lummo.Infrastructure.Validators;

public class SignInDetailsValidator : AbstractValidator<SignInDetails>
{
    public SignInDetailsValidator(IOptions<ValidationSettings> validationSettings)
    {
        var validationSettingsValue = validationSettings.Value;
        RuleFor(sign => sign.EmailAddress).NotEmpty().Matches(validationSettingsValue.EmailAddressRegexPattern);
        RuleFor(sign => sign.Password).NotEmpty();
    }
}
