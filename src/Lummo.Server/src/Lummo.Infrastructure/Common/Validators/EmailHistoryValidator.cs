using FluentValidation;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Infrastructure.Common.Validators;

public class EmailHistoryValidator : AbstractValidator<EmailHistory>
{
    public EmailHistoryValidator()
    {
        RuleSet(
            EntityEvent.OnCreate.ToString(),
            () =>
            {
                RuleFor(history => history.Content).NotEmpty().MaximumLength(129_536);
                RuleFor(history => history.SenderEmailAddress).NotEmpty().MaximumLength(64);
                RuleFor(history => history.ReceiverEmailAddress).NotEmpty().MaximumLength(64);
                RuleFor(history => history.Subject).NotEmpty();
                RuleFor(history => history.Type).NotEqual(NotificationType.Email);
            });
    }
}
