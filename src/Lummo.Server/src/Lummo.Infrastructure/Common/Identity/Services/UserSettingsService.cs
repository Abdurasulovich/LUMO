using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Entities;
using Lummo.Persistence.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Lummo.Infrastructure.Common.Identity.Services;

public class UserSettingsService(IUserSettingsRepository userSettingsRepository) : IUserSettingsService
{
    public ValueTask<UserSettings> CreateAsync(UserSettings userSettings, bool saveChanges = true, CancellationToken cancellationToken = default)
        => userSettingsRepository.CreateAsync(userSettings, saveChanges, cancellationToken);

    public ValueTask<bool> DeleteAsync(UserSettings userSettings, bool saveChanges = true, CancellationToken cancellationToken = default)
        => userSettingsRepository.DeleteAsync(userSettings, saveChanges, cancellationToken);

    public ValueTask<bool> DeleteByIdAsync(Guid userSettingsId, bool saveChanges = true, CancellationToken cancellationToken = default)
        => userSettingsRepository.DeleteByIdAsync(userSettingsId, saveChanges, cancellationToken);

    public IQueryable<UserSettings> Get(Expression<Func<UserSettings, bool>>? predicate, bool asNoTracking = false)
        => userSettingsRepository.Get(predicate, asNoTracking);

    public ValueTask<UserSettings?> GetByIdAsync(Guid userSettingsId, bool asNoTracking = false, CancellationToken cancellationToken = default)
        => userSettingsRepository.GetByIdAsync(userSettingsId, asNoTracking, cancellationToken);

    public ValueTask<IList<UserSettings>> GetByIdsAsync(IEnumerable<Guid> ids, bool asNoTracking = false, CancellationToken cancellationToken = default)
        => userSettingsRepository.GetByIdsAsync(ids, asNoTracking, cancellationToken);

    public ValueTask<UserSettings> UpdateAsync(UserSettings userSettings, bool saveChanges = true, CancellationToken cancellationToken = default)
        => userSettingsRepository.UpdateAsync(userSettings, saveChanges, cancellationToken);
}
