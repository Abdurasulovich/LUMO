using FluentValidation;
using Lummo.Application.Common.Notifications.Events;

namespace Lummo.Infrastructure.Validators;

public class ProcessNotificationEventValidator : AbstractValidator<ProcessNotificationEvent>
{
    public ProcessNotificationEventValidator()
    {
        RuleFor(process => process.ReceiverUserId).NotEqual(Guid.Empty);
    }
}
