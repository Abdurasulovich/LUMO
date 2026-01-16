using Lummo.Domain.Common.Query;
using Lummo.Domain.Entities;
using Lummo.Domain.Enums;

namespace Lummo.Application.Common.Notifications.Services.Interfaces;

public interface IEmailTemplateService
{
    ValueTask<IList<EmailTemplate>> GetByFilterAsync(
        FilterPagination filterPagination,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
        );

    ValueTask<EmailTemplate?> GetByTypeAsync(
        NotificationTemplateType templateType,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
        );

    ValueTask<EmailTemplate> CreateAsync(
        EmailTemplate emailTemplate,
        bool saveChanges = true,
        CancellationToken cancellationToken = default
        );
}
