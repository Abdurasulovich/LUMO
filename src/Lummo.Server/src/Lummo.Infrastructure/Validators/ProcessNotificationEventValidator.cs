using FluentValidation;
using Lummo.Domain.Common.Events;

namespace Lummo.Infrastructure.Validators;

public class ProcessNotificationEventValidator : AbstractValidator<ProcessNotificationEvent>
{
    public ProcessNotificationEventValidator()
    {
        RuleFor(process => process.ReceiverUserId).NotEqual(Guid.Empty);
    }
}
