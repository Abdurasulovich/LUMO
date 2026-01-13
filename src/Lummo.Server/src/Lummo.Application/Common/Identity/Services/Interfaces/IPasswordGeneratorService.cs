using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IPasswordGeneratorService
{
    string GeneratePassword();

    string GetValidatePassword(string password, User user);
}
