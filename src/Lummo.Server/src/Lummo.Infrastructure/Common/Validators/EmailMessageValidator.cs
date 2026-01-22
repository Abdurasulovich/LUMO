using FluentValidation;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Domain.Enums;

namespace Lummo.Infrastructure.Common.Validators;

public class EmailMessageValidator : AbstractValidator<EmailMessage>
{
    public EmailMessageValidator()
    {
        RuleSet(
            NotificationEvent.OnRendering.ToString(),
            () =>
            {
                RuleFor(message => message.Template).NotNull();
                RuleFor(message => message.Variables).NotNull();
                RuleFor(message => message.Template.Content).NotNull().NotEmpty();
            });

        RuleSet(
            NotificationEvent.OnSending.ToString(),
            () =>
            {
                RuleFor(message => message.SenderEmailAddress).NotNull().NotEmpty();
                RuleFor(message => message.ReceiverEmailAddress).NotNull()
                    .NotEmpty();
                RuleFor(message => message.Subject).NotNull().NotEmpty();
                RuleFor(message => message.Body).NotNull().NotEmpty();
            });
    }
}
