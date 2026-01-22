using AutoMapper;
using Lummo.Domain.Entities;
using Lummo.Server.Models.DTOs;

namespace Lummo.Server.Mappers;

public class IdentityTokenMapper : Profile
{
    public IdentityTokenMapper()
    {
        CreateMap<AccessToken, AccessTokenDto>();

        CreateMap<(AccessToken AccessToken, RefreshToken RefreshToken), IdentityTokenDto>()
            .ForMember(dest=>dest.AccessToken, opt=>opt.MapFrom(src=>src.AccessToken.Token))
            .ForMember(dest=>dest.RefreshToken, opt=>opt.MapFrom(src=>src.RefreshToken.Token));
    }
}
