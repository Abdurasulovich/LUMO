using AutoMapper;
using Lummo.Domain.Entities;
using Lummo.Server.Models.DTOs;

namespace Lummo.Server.Mappers;

public class EmailTemplateMapper : Profile
{
    public EmailTemplateMapper()
    {
        CreateMap<EmailTemplate, EmailTemplateDto>().ReverseMap();
    }
}
