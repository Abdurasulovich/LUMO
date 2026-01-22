using FluentValidation;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Infrastructure.Common.Validators;

public class EmailTemplateValidator : AbstractValidator<EmailTemplate>
{
    public EmailTemplateValidator()
    {
        RuleSet(EntityEvent.OnCreate.ToString(),
        () =>
        {

            RuleFor(template => template.Content)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(129_536);

            RuleFor(template => template.Type)
            .Equal(NotificationType.Email);

            RuleFor(template => template.Subject)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(129_536);
        });
    }
}
