using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IAccessTokenGeneratorService
{
    AccessToken GetToken(User user);

    Guid GetTokenId(string accessToken);
}
