using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories;

public class UserInfoVerificationCodeRepository(IdentityDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<UserInfoVerificationCode, IdentityDbContext>(dbContext, cacheBroker),
    IUserInfoVerificationCodeRepository
{
    public new IQueryable<UserInfoVerificationCode> Get(Expression<Func<UserInfoVerificationCode, bool>> predicate, bool asNoTracking)
    {
        return Get(predicate, asNoTracking);
    }

    public new ValueTask<UserInfoVerificationCode?> GetByIdAsync(Guid codeId, bool asNoTracking, CancellationToken cancellationToken)
    {
        return GetByIdAsync(codeId, asNoTracking, cancellationToken);
    }

    public new async ValueTask<UserInfoVerificationCode> CreateAsync(UserInfoVerificationCode userInfoVerificationCode, bool saveChanges, CancellationToken cancellationToken)
    {
        await DbContext.UserInfoVerificationCodes.Where(code =>
        code.UserId == userInfoVerificationCode.UserId && code.CodeType == userInfoVerificationCode.CodeType)
            .ExecuteUpdateAsync(setter => setter.SetProperty(code => code.IsActive, false), cancellationToken);

        return await base.CreateAsync(userInfoVerificationCode, saveChanges, cancellationToken);
    }
    public async ValueTask DeactivateAsync(Guid codeId, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        await DbContext.UserInfoVerificationCodes.Where(code => code.Id == codeId)
            .ExecuteUpdateAsync(setter => setter.SetProperty(codeId => codeId.IsActive, false), cancellationToken);

        if (saveChanges)
            await DbContext.SaveChangesAsync(cancellationToken);
    }
}