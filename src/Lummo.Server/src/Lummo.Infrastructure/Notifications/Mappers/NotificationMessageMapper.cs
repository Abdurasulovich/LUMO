using AutoMapper;
using Lummo.Application.Common.Notifications.Models;

namespace Lummo.Infrastructure.Notifications.Mappers;

public class NotificationMessageMapper : Profile
{
    public NotificationMessageMapper()
    {
        CreateMap<EmailProcessNotificationEvent, EmailMessage>();
    }
}
