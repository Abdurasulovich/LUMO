using Lummo.Mobile.Services.Identity.Interfaces;
using Lummo.Mobile.Services.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Lummo.Mobile.Services.Identity.Implements;

public class UserService(DatabaseService databaseService) : IUserService
{
    public async Task AddUserAsync(User user)
    {
        await DeleteUserAsync();
        await databaseService.AddAsync(user);
    }

    public async Task DeleteUserAsync()
    {
        var users = await databaseService.GetAllAsync<User>();
        foreach (var user in users)
            await databaseService.DeleteAsync(user);
    }

    public async Task<User?> GetUserAsync()
    {
        var users = await databaseService.GetAllAsync<User>();
        return users.FirstOrDefault();
    }

    public async ValueTask<Guid?> GetUserIdFromAccessToken(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);

        var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "UserId");

        if (userIdClaim == null)
            return null;

        return Guid.Parse(userIdClaim.Value); // endi to‘g‘ri
    }

    public async Task UpdateUserAsync(User user)
    {
        var existing = await GetUserAsync();
        if (existing != null)
        {
            user.Id = existing.Id;
            await databaseService.UpdateAsync(user);
        }
        else
        {
            await databaseService.AddAsync(user);
        }
    }
}
