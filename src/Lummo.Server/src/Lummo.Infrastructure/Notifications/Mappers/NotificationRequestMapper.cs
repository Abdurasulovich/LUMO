using AutoMapper;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Domain.Common.Events;

namespace Lummo.Infrastructure.Notifications.Mappers;

public class NotificationRequestMapper : Profile
{
    public NotificationRequestMapper()
    {
        CreateMap<ProcessNotificationEvent, EmailProcessNotificationEvent>();
    }
}
