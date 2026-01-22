using FluentValidation;
using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using Lummo.Infrastructure.Common.Validators;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lummo.Infrastructure.Common.Identity.Services;

public class UserService(IUserRepository userRepository, UserValidator userValidator) : IUserService
{
    public async ValueTask<User> CreateAsync(User user, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        var validationResult = userValidator
            .Validate(user,
                options =>
                    options.IncludeRuleSets(EntityEvent.OnCreate.ToString()));

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return await userRepository.CreateAsync(user, saveChanges, cancellationToken);
    }

    public async ValueTask<bool> DeleteAsync(User user, bool saveChanges = true, CancellationToken cancellationToken = default)
        => await userRepository.DeleteAsync(user, saveChanges, cancellationToken);

    public async ValueTask<bool> DeleteByIdAsync(Guid userId, bool saveChanges = true, CancellationToken cancellationToken = default)
        => await userRepository.DeleteByIdAsync(userId, saveChanges, cancellationToken);

    public IQueryable<User> Get(Expression<Func<User, bool>>? predicate = null, bool asNoTracking = false)
        => userRepository.Get(predicate, asNoTracking);

    public async ValueTask<User?> GetByEmailAddressAsync(string emailAddress, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var user = userRepository
         .Get(asNoTracking: asNoTracking)
         .Include(user => user.Roles);

        return await user.FirstOrDefaultAsync(user => user.EmailAddress == emailAddress, cancellationToken: cancellationToken);

    }
    public ValueTask<User?> GetByIdAsync(Guid userId, bool asNoTracking = false, CancellationToken cancellationToken = default)
        => userRepository.GetByIdAsync(userId, asNoTracking, cancellationToken);

    public ValueTask<IList<User>> GetByIdsAsync(IEnumerable<Guid> ids, bool asNoTracking = false, CancellationToken cancellationToken = default)
        => userRepository.GetByIdsAsync(ids, asNoTracking, cancellationToken);

    public ValueTask<User> UpdateAsync(User user, bool saveChanges = true, CancellationToken cancellationToken = default)
        => userRepository.UpdateAsync(user, saveChanges, cancellationToken);
}
