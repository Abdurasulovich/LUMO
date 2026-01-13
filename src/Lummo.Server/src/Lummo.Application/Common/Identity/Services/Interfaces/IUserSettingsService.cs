using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Identity.Services.Interfaces;

public interface IUserSettingsService
{
    ValueTask<UserSettings?> GetByIdAsync(Guid userSettingsId, bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<UserSettings> CreateAsync(UserSettings userSettings, bool saveChanges = true,
        CancellationToken cancellationToken = default);
}
