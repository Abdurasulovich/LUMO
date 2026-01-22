using AutoMapper;
using Lummo.Domain.Entities;
using Lummo.Server.Models.DTOs;

namespace Lummo.Server.Mappers;

public class UserSettingsMapper : Profile
{
    public UserSettingsMapper()
    {
        CreateMap<UserSettings, UserSettingsDto>().ReverseMap();
    }
}
