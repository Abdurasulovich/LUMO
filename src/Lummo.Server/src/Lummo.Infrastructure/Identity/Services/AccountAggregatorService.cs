using Lummo.Application.Common.EventBus.Brokers.Interfaces;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Domain.Common.Events;
using Lummo.Domain.Common.Verifications.Services.Interfaces;
using Lummo.Domain.Constants;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Infrastructure.Identity.Services;

public class AccountAggregatorService(
    IUserService userService,
    IUserSettingsService userSettingsService,
    IUserInfoVerificationCodeService userInfoVerificationCodeService,
    IEventBusBroker eventBusBroker)
    : IAccountAggregatorService
{
    public async ValueTask<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        user.Role = RoleType.User;
        var createUser = await userService.CreateAsync(user, cancellationToken: cancellationToken);
        await userSettingsService.CreateAsync(
            new UserSettings
            {
                Id = createUser.Id
            }, cancellationToken: cancellationToken
            );

        //send welcome email event
        var welcomeNotificationEvent = new ProcessNotificationEvent
        {
            ReceiverUserId = createUser.Id,
            TemplateType = NotificationTemplateType.WelcomeNotification,
            Variables = new Dictionary<string, string>
            {
                { NotificationTemplateConstants.UserNamePlaceholder, createUser.FirstName }
            }
        };

        //send verification email event
        await eventBusBroker.PublishAsync(
            welcomeNotificationEvent,
            EventBusConstants.NotificationExchangeName,
            EventBusConstants.ProcessNotificationQueueName,
            cancellationToken: cancellationToken
            );

        var verificationCode = await userInfoVerificationCodeService.CreateAsync(
            VerificationCodeType.EmailAddressVerification,
            createUser.Id,
            cancellationToken
            );

        //send verification email event
        var sendVerificationEmail = new EmailProcessNotificationEvent
        {
            ReceiverUserId = createUser.Id,
            TemplateType = NotificationTemplateType.EmailAddressVerificationNotification,
            Variables = new Dictionary<string, string>
            {
                {
                    NotificationTemplateConstants.EmailAddressVerificationLinkPlaceholder,
                    verificationCode.VerificationLink
                }
            }
        };

        await eventBusBroker.PublishAsync(
            sendVerificationEmail,
            EventBusConstants.NotificationExchangeName,
            EventBusConstants.ProcessNotificationQueueName,
            cancellationToken: cancellationToken
            );

        return true;
    }
}
