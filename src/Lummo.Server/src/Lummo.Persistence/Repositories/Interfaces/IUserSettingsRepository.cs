using Lummo.Domain.Entities;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories.Interfaces;

public interface IUserSettingsRepository
{
    IQueryable<UserSettings> Get(Expression<Func<UserSettings, bool>>? predicate,
        bool asNoTracking = false);

    ValueTask<UserSettings?> GetByIdAsync(Guid userId,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);
    ValueTask<IList<UserSettings>> GetByIdsAsync(IEnumerable<Guid> ids,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);
    ValueTask<UserSettings> CreateAsync(UserSettings userSettings,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<UserSettings> UpdateAsync(UserSettings userSettings,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    ValueTask<UserSettings?> DeleteAsync(UserSettings userSettings,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
    ValueTask<UserSettings?> DeleteByIdAsync(Guid userSettingsId,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
}
