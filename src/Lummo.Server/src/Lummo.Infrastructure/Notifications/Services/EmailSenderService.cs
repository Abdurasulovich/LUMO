using FluentValidation;
using Lummo.Application.Common.Notifications.Brokers.Interfaces;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Domain.Enums;
using Lummo.Domain.Extensions;

namespace Lummo.Infrastructure.Notifications.Services;

public class EmailSenderService(IValidator<EmailMessage> emailMessageValidator,
    IEnumerable<IEmailSenderBroker> emailSenderBrokers) : IEmailSenderService
{
    private readonly IValidator<EmailMessage> _emailMessageValidator = emailMessageValidator;
    private readonly IEnumerable<IEmailSenderBroker> _emailSenderBrokers = emailSenderBrokers;
    public async ValueTask<bool> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        var validationResult = _emailMessageValidator.Validate(
            emailMessage,
            options => options.IncludeRuleSets(NotificationProcessingEvent.OnSending.ToString())
            );

        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        foreach(var emailSenderBroker in _emailSenderBrokers)
        {
            var sendNotificationTask = () => emailSenderBroker.SendAsync(emailMessage, cancellationToken);
            var result = await sendNotificationTask.GetValueAsync();

            emailMessage.IsSuccessful = result.IsSuccess;
            emailMessage.ErrorMessage = result.Exception?.Message;
            return result.IsSuccess;
        }
        return false;

    }
}
