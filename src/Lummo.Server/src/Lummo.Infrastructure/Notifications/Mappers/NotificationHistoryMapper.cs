using AutoMapper;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Domain.Entities;

namespace Lummo.Infrastructure.Notifications.Mappers;

public class NotificationHistoryMapper : Profile
{
    public NotificationHistoryMapper()
    {
        CreateMap<EmailMessage, EmailHistory>()
            .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.Template.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Body));
    }
}
