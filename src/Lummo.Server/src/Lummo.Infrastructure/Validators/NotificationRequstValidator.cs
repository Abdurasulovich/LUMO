using FluentValidation;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Application.Common.Notifications.Events;
using Lummo.Domain.Enums;

namespace Lummo.Infrastructure.Validators;

public class NotificationRequstValidator : AbstractValidator<ProcessNotificationEvent>
{
    public NotificationRequstValidator(IUserService userService)
    {
        var templateRequireSender = new List<NotificationTemplateType>
        {
            NotificationTemplateType.ReferrelNotification
        };

        RuleFor(request => request.SenderUserId)
            .NotEqual(Guid.Empty)
            .NotNull()
            .When(request => templateRequireSender.Contains(request.TemplateType))
            .CustomAsync(
            async (senderUserId, context, cancellationToken) =>
            {
                var user = await userService.GetByIdAsync(senderUserId, true, cancellationToken);
                if (user is null)
                    context.AddFailure("Sender user not found.");
            }
        );

        RuleFor(request => request.ReceiverUserId)
            .NotEqual(Guid.Empty)
            .CustomAsync(
            async (receiverUserId, context, cancellationToken) =>
            {
                var user = await userService.GetByIdAsync(receiverUserId, true, cancellationToken);
                if (user is null)
                    context.AddFailure("Receiver user not found.");
            });
    }
}
