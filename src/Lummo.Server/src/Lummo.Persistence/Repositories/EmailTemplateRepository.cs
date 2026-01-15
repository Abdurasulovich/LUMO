using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories;

public class EmailTemplateRepository(NotificationDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<EmailTemplate, NotificationDbContext>(dbContext, cacheBroker), IEmailTemplateRepository
{
    public new ValueTask<EmailTemplate> CreateAsync(EmailTemplate emailTemplate, bool saveChanges, CancellationToken cancellationToken)
    {
        return CreateAsync(emailTemplate, saveChanges, cancellationToken);
    }

    public new IQueryable<EmailTemplate> Get(Expression<Func<EmailTemplate, bool>> predicate, bool asNoTracking)
    {
        return Get(predicate, asNoTracking);
    }
}
