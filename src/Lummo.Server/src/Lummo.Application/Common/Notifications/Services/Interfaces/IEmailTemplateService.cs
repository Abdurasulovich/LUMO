using Lummo.Domain.Entities;
using Lummo.Domain.Enums;
using System.Linq.Expressions;

namespace Lummo.Application.Common.Notifications.Services.Interfaces;

public interface IEmailTemplateService
{
    IQueryable<EmailTemplate> Get(Expression<Func<EmailTemplate, bool>>? predicate = default,
        bool asNoTracking = false);

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
