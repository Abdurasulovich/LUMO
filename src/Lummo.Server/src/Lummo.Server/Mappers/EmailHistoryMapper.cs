using AutoMapper;
using Lummo.Application.Common.Notifications.Models;
using Lummo.Domain.Entities;

namespace Lummo.Server.Mappers;

public class EmailHistoryMapper : Profile
{
    public EmailHistoryMapper()
    {
        CreateMap<EmailMessage, EmailHistory>().ReverseMap();
    }
}
