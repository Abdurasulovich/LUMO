using FluentValidation;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Infrastructure.Validators;

public class EmailTemplateValidator : AbstractValidator<EmailTemplate>
{
    public EmailTemplateValidator()
    {
        RuleFor(template => template.Content)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(256);

        RuleFor(template => template.Type).Equal(NotificationType.Email);
    }
}
