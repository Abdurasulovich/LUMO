using AutoMapper;
using Lummo.Domain.Entities;
using Lummo.Server.Models.DTOs;

namespace Lummo.Server.Mappers;

public class AccessTokenMapper : Profile
{
    public AccessTokenMapper()
    {
        CreateMap<AccessToken, AccessTokenDto>().ReverseMap();
    }
}
