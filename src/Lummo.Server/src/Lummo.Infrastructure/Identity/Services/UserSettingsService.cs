using Lummo.Application.Common.Identity.Services.Interfaces;
using Lummo.Domain.Entities;
using Lummo.Persistence.Repositories.Interfaces;

namespace Lummo.Infrastructure.Identity.Services;

public class UserSettingsService(IUserSettingsRepository userSettingsRepository) : IUserSettingsService
{
    public ValueTask<UserSettings> CreateAsync(UserSettings userSettings, bool saveChanges = true, CancellationToken cancellationToken = default)
    {
        return userSettingsRepository.CreateAsync(userSettings, saveChanges, cancellationToken);
    }

    public ValueTask<UserSettings?> GetByIdAsync(Guid userSettingsId, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        return userSettingsRepository.GetByIdAsync(userSettingsId, asNoTracking, cancellationToken);
    }
}
