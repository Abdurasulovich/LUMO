using AutoMapper;
using FluentValidation;
using Lummo.Application.Common.EventBus.Brokers.Interfaces;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Application.Common.Notifications.Events;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Application.Common.Notifications.Services.Interfaces;
using Lummo.Application.Common.Serializer;
using Lummo.Domain.Common.Events;
using Lummo.Domain.Common.Query;
using Lummo.Domain.Constants;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Domain.Extensions;
using Lummo.Infrastructure.Common.EventBus.Services;
using Lummo.Infrastructure.Settings;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;

namespace Lummo.Infrastructure.Notifications.EventSubscriber;

public class NotificationSubscriber(
    IServiceScopeFactory serviceScopeFactory,
    IMapper mapper,
    IJsonSerializationSettingsProvider jsonSerializationSettingsProvider,
    IOptions<NotificationSubscriberSettings> notificationSubscriberSettings,
    IRabbitMqConnectionProvider rabbitMqConnectionProvider,
    IEventBusBroker eventBusBroker,
    IOptions<NotificationSettings> notificationSettings
    ) : EventSubscriber<NotificationEvent>(
        rabbitMqConnectionProvider,
        notificationSubscriberSettings,
        [EventBusConstants.ProcessNotificationQueueName, EventBusConstants.RenderNotificationQueueName, EventBusConstants.SendNotificationQueueName],
        jsonSerializationSettingsProvider
        )
{
    private readonly NotificationSettings _notificationSettings = notificationSettings.Value;

    protected override async ValueTask SetChannelAsync()
    {
        await base.SetChannelAsync();
        await Channel.ExchangeDeclareAsync(EventBusConstants.NotificationExchangeName, ExchangeType.Direct, true);

        await Channel.QueueDeclareAsync(EventBusConstants.ProcessNotificationQueueName, true, false, false);
        await Channel.QueueDeclareAsync(EventBusConstants.RenderNotificationQueueName, true, false, false);
        await Channel.QueueDeclareAsync(EventBusConstants.SendNotificationQueueName, true, false, false);

        await Channel.QueueBindAsync(
            EventBusConstants.ProcessNotificationQueueName,
            EventBusConstants.NotificationExchangeName,
            EventBusConstants.ProcessNotificationQueueName
            );

        await Channel.QueueBindAsync(
            EventBusConstants.RenderNotificationQueueName,
            EventBusConstants.NotificationExchangeName,
            EventBusConstants.RenderNotificationQueueName
            );

        await Channel.QueueBindAsync(
            EventBusConstants.SendNotificationQueueName,
            EventBusConstants.NotificationExchangeName,
            EventBusConstants.SendNotificationQueueName
            );
    }

    protected override async ValueTask<(bool Result, bool Redeliver)> ProcessAsync(NotificationEvent @event, 
        CancellationToken cancellationToken)
    {
        var eventHandler = () => @event switch
        {
            ProcessNotificationEvent processNotificationEvent => ProcessNotificationAsync(processNotificationEvent,
            cancellationToken),
            RenderNotificationEvent renderNotifiationEvent => RenderedNotificationAsync(renderNotifiationEvent,
            cancellationToken),
            SenderNotificationEvent senderNotificationEvent => SendNotificationAsync(senderNotificationEvent, 
            cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(@event))
        };

        //declare exchange and queues
        var result = await eventHandler.GetValueAsync();
        return (Result: result.Data, Redeliver: false);
    }

    private async ValueTask ProcessNotificationAsync(ProcessNotificationEvent processNotificationEvent, 
        CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
        var processNotifiactionEventValidator = 
            scope.ServiceProvider.GetRequiredService<IValidator<ProcessNotificationEvent>>();

        var validdationResult =
            await processNotifiactionEventValidator.ValidateAsync(processNotificationEvent, cancellationToken);
        if (!validdationResult.IsValid)
            throw new ValidationException(validdationResult.Errors);

        var senderUser = processNotificationEvent.SenderUserId != Guid.Empty
            ? await userService.GetByIdAsync(processNotificationEvent.SenderUserId, cancellationToken: cancellationToken)
            : await userService.GetSystemUserAsync(true, cancellationToken:cancellationToken);

        var receiverUserQuery = new QuerySpecification<User>(1, 1, true);
        receiverUserQuery.FilteringOptions.Add(user=>user.Id.Equals(processNotificationEvent.ReceiverUserId));
        receiverUserQuery.IncludingOptions.Add(user=>user.UserSettings!);

        var receiverUser = (await userService.GetAsync(receiverUserQuery, cancellationToken)).First();

        if (!processNotificationEvent.Type.HasValue && receiverUser!.UserSettings!.PreferredNotificationType.HasValue)
            processNotificationEvent.Type = receiverUser!.UserSettings.PreferredNotificationType!.Value;

        if(!processNotificationEvent.Type.HasValue)
            processNotificationEvent.Type = _notificationSettings.DefaultNotificationType;

        var renderNotificationEvent = new RenderNotificationEvent
        {
            SenderUserId = senderUser!.Id,
            ReceiverUserId = receiverUser.Id,
            Template = (await emailTemplateService.GetByTypeAsync(processNotificationEvent.TemplateType,
            cancellationToken: cancellationToken))!,
            SenderUser = senderUser,
            ReceiverUser = receiverUser,
            Variables = processNotificationEvent.Variables ?? new Dictionary<string, string>()
        };

        await eventBusBroker.PublishAsync(
            renderNotificationEvent,
            EventBusConstants.NotificationExchangeName,
            EventBusConstants.RenderNotificationQueueName,
            cancellationToken: cancellationToken
            );
    }

    private async ValueTask RenderedNotificationAsync(RenderNotificationEvent renderNotificationEvent, 
        CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var emailRenderingService = scope.ServiceProvider.GetRequiredService<IEmailRenderingService>();

        if(renderNotificationEvent.Template.Type == NotificationType.Email)
        {
            var emailMessage = new EmailMessage
            {
                SenderEmailAddress = renderNotificationEvent.SenderUser.EmailAddress,
                ReceiverEmailAddress = renderNotificationEvent.ReceiverUser.EmailAddress,
                EmailTemplate = (EmailTemplate)renderNotificationEvent.Template,
                Variables = renderNotificationEvent.Variables
            };

            await emailRenderingService.RenderAsync(emailMessage, cancellationToken);

            var sendNotificationEvent = new SenderNotificationEvent
            {
                SenderUserId = renderNotificationEvent.SenderUser.Id,
                ReceiverUserId = renderNotificationEvent.ReceiverUser.Id,
                Message = emailMessage
            };

            await eventBusBroker.PublishAsync(
                sendNotificationEvent,
                EventBusConstants.NotificationExchangeName,
                EventBusConstants.SendNotificationQueueName,
                cancellationToken: cancellationToken
                );
        }
    }

    private async ValueTask SendNotificationAsync(SenderNotificationEvent senderNotificationEvent,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var emailSenderService = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();
        var emailHistoryService = scope.ServiceProvider.GetRequiredService<IEmailHistoryService>();

        if(senderNotificationEvent.Message is EmailMessage emailMessage)
        {
            await emailSenderService.SendAsync(emailMessage, cancellationToken);

            var history = mapper.Map<EmailHistory>(emailMessage);
            history.SenderUserId = senderNotificationEvent.SenderUserId;
            history.ReceiverUserId = senderNotificationEvent.ReceiverUserId;

            await emailHistoryService.CreateAsync(history, cancellationToken: cancellationToken);
            if (!history.IsSuccessful) throw new InvalidOperationException("Email history is not created");
        }
    }
}
