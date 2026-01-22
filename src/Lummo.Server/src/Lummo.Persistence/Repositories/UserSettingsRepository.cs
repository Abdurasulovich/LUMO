using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;
using System.Linq.Expressions;
namespace Lummo.Persistence.Repositories;

public class UserSettingsRepository(AppDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<UserSettings, AppDbContext>(dbContext, cacheBroker, new())
    , IUserSettingsRepository
{
    public new IQueryable<UserSettings> Get(Expression<Func<UserSettings, bool>>? predicate, bool asNoTracking)
        => base.Get(predicate, asNoTracking);

    public new ValueTask<IList<UserSettings>> GetByIdsAsync(IEnumerable<Guid> ids, bool asNoTracking, CancellationToken cancellationToken)
        => base.GetByIdsAsync(ids, asNoTracking, cancellationToken);

    public new ValueTask<bool> DeleteAsync(UserSettings userSettings, bool saveChanges, CancellationToken cancellationToken)
        => base.DeleteAsync(userSettings, saveChanges, cancellationToken);

    public new ValueTask<bool> DeleteByIdAsync(Guid userSettingsId, bool saveChanges, CancellationToken cancellationToken)
        => base.DeleteByIdAsync(userSettingsId, saveChanges, cancellationToken);

    public new ValueTask<UserSettings?> GetByIdAsync(Guid userId, bool asNoTracking, CancellationToken cancellationToken)
        => base.GetByIdAsync(userId, asNoTracking, cancellationToken);

    public new ValueTask<UserSettings> CreateAsync(UserSettings userSettings, bool saveChanges, CancellationToken cancellationToken)
        => base.CreateAsync(userSettings, saveChanges, cancellationToken);

    public new ValueTask<UserSettings> UpdateAsync(UserSettings userSettings, bool saveChanges, CancellationToken cancellationToken)
    {
        var foundUserSettings = dbContext.UserSettings.SingleOrDefault(dbUserSettings => dbUserSettings.Id == userSettings.Id)
            ?? throw new InvalidOperationException("User settings not found with this Id!");

        foundUserSettings.PreferredTheme = userSettings.PreferredTheme;
        foundUserSettings.PreferredNotificationType = userSettings.PreferredNotificationType;

        return base.UpdateAsync(foundUserSettings, saveChanges, cancellationToken);
    }
}