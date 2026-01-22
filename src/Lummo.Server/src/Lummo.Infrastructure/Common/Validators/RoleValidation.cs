using FluentValidation;
using Lummo.Application.Common.Settings;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Lummo.Infrastructure.Common.Validators;

public class RoleValidation : AbstractValidator<Role>
{
    public RoleValidation(IOptions<ValidationSettings> validationSettings)
    {
        var validationSettingsValue = validationSettings.Value;

        RuleSet(
            EntityEvent.OnCreate.ToString(),
            () =>
            {
                RuleFor(code => code.Id).NotEqual(Guid.Empty);
                RuleFor(code => code.Type).NotEmpty();
            }
        );
    }
}
