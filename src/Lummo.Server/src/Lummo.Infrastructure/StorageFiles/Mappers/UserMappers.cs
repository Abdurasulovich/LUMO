using AutoMapper;
using Lummo.Application.Common.Identity.Models;
using Lummo.Domain.Entities;

namespace Lummo.Infrastructure.StorageFiles.Mappers;

public class UserMappers : Profile
{
    public UserMappers()
    {
        CreateMap<SignUpDetails, User>();
    }
}
