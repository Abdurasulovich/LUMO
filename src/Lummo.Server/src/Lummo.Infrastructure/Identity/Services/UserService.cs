using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Lummo.Infrastructure.Identity.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly string _folderPath = "Assets/User/";
    public async ValueTask<User> CreateAsync(User user, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        return await userRepository.CreateAsync(user, saveChanges, cancellationToken);
    }

    public IQueryable<User> Get(Expression<Func<User, bool>>? predicate = null, bool asNoTracking = false)
    {
        return userRepository.Get(predicate, asNoTracking);
    }

    public ValueTask<IList<User>> GetAsync(QuerySpecification<User> querySpecification, CancellationToken cancellationToken = default)
    {
        return userRepository.GetAsync(querySpecification, cancellationToken);
    }

    public ValueTask<User?> GetByIdAsync(Guid userId, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        return userRepository.GetByIdAsync(userId, asNoTracking, cancellationToken);
    }

    public async ValueTask<Guid?> GetIdByEmailAddressAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        var userId = await Get(user => user.EmailAddress == emailAddress, true).Select(user => user.Id)
            .FirstOrDefaultAsync(cancellationToken);
        return userId != Guid.Empty ? userId : default(Guid?);
    }

    public async ValueTask<User> GetSystemUserAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        return await Get(user => user.Role == RoleType.System, asNoTracking).FirstAsync(cancellationToken);
    }

    public ValueTask<User> UpdateAsync(User user, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        return userRepository.UpdateAsync(user, saveChanges, cancellationToken);
    }

    public async ValueTask<string> UploadImageAsync(Guid id, IFormFile imagePath, string webRootPath, CancellationToken cancellationToken = default)
    {
        var findFile = await GetByIdAsync(id, cancellationToken: cancellationToken) ??
            throw new InvalidOperationException("User with this id not found!");

        var relativePath = _folderPath + id.ToString() + "." + imagePath.FileName.Split('.')[1];
        var filePath = Path.Combine(webRootPath, relativePath);

        if (File.Exists(filePath)) File.Delete(filePath);

        using(var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imagePath.CopyToAsync(fileStream, cancellationToken);
        }

        findFile.ImageUrl = relativePath;
        await UpdateAsync(findFile, cancellationToken: cancellationToken);
        return relativePath;
    }
}
