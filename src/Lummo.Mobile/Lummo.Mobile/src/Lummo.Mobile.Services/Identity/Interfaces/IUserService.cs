using Lummo.Mobile.Services.Models;

namespace Lummo.Mobile.Services.Identity.Interfaces;

public interface IUserService
{
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync();
    Task<User?> GetUserAsync();

    ValueTask<Guid?> GetUserIdFromAccessToken(string accessToken);
}
