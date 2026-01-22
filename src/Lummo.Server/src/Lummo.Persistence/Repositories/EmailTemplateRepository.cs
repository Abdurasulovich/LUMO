using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.Caching.Models;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories;

public class EmailTemplateRepository(AppDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<EmailTemplate, AppDbContext>(dbContext, cacheBroker, new CacheEntryOptions())
    , IEmailTemplateRepository
{
    public new ValueTask<EmailTemplate> CreateAsync(EmailTemplate emailTemplate, bool saveChanges, CancellationToken cancellationToken)
    => base.CreateAsync(emailTemplate, saveChanges, cancellationToken);

    public new IQueryable<EmailTemplate> Get(Expression<Func<EmailTemplate, bool>>? predicate, bool asNoTracking)
    => base.Get(predicate, asNoTracking);
}
