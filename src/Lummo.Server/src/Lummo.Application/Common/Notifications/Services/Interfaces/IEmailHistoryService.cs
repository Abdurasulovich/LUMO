using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;

namespace Lummo.Application.Common.Notifications.Services.Interfaces;

public interface IEmailHistoryService
{
    ValueTask<IList<EmailHistory>> GetByFilterAsync(
        FilterPagination paginationOptions,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    ValueTask<EmailHistory> CreateAsync(
        EmailHistory emailHistory,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
}
