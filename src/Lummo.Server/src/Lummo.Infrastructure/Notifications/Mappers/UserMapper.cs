using AutoMapper;
using Lummo.Application.Common.Identity.Models;
using Lummo.Domain.Entities;

namespace Lummo.Infrastructure.Notifications.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<SignUpDetails, User>();
    }
}
