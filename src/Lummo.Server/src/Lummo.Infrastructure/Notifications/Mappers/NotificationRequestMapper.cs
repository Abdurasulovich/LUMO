using AutoMapper;
using Lummo.Application.Common.Notifications.Events;
using Lummo.Application.Common.Notifications.Models;

namespace Lummo.Infrastructure.Notifications.Mappers;

public class NotificationRequestMapper : Profile
{
    public NotificationRequestMapper()
    {
        CreateMap<ProcessNotificationEvent, EmailProcessNotificationEvent>();
    }
}
