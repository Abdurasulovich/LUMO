using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Peristence.Repositories.Interfaces;
    namespace Lummo.Persistence.Repositories;

    public class UserSettingsRepository(IdentityDbContext dbContext, ICacheBroker cacheBroker)
        : EntityRepositoryBase<UserSettings, IdentityDbContext>(dbContext, cacheBroker), IUserSettingsRepository
{
    public new ValueTask<UserSettings?> GetByIdAsync(Guid userId, bool asNoTracking = false, 
        CancellationToken cancellationToken = default)
        => base.GetByIdAsync(userId, asNoTracking, cancellationToken);

    public new ValueTask<UserSettings> CreateAsync(UserSettings userSettings, bool saveChanges = true,
        CancellationToken cancellationToken = default)
        => base.CreateAsync(userSettings, saveChanges, cancellationToken);
}