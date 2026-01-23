using FluentValidation;
using Lummo.Application.Common.Notifications.Brokers.Interfaces;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Domain.Enums;
using Lummo.Domain.Extensions;
using Lummo.Persistence.Extensions;

namespace Lummo.Infrastructure.Common.Notifications.Services;

public class EmailSenderService(IEnumerable<IEmailSenderBroker> emailSenderBroker,
    IValidator<EmailMessage> emailMessageValidator
    ) : IEmailSenderService
{
    public async ValueTask<bool> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        var validationResult = emailMessageValidator.Validate(
            emailMessage,
            options => options.IncludeRuleSets(NotificationEvent.OnSending.ToString())
            );

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var accomplishedBroker = await emailSenderBroker.FirstOrDefaultAsync(async emailSenderBrokers =>
        {
            var sendNotifcationTask = () => emailSenderBrokers.SendAsync(emailMessage, cancellationToken);
            var result = await sendNotifcationTask.GetValueAsync();
            (emailMessage.IsSuccessful, emailMessage.ErrorMessage) = (result.IsSuccess, result.Exception?.Message);

            return result.IsSuccess;
        }, cancellationToken);
        return accomplishedBroker is not null;

    }
}
