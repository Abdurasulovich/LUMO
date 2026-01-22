using AutoMapper;
using Lummo.Domain.Entities;
using Lummo.Server.Dtos;

namespace Lummo.Server.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}
