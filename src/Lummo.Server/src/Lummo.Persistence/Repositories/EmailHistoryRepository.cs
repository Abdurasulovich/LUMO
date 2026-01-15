using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories;

public class EmailHistoryRepository : EntityRepositoryBase<EmailHistory, NotificationDbContext>, IEmailHistoryRepository
{
    public EmailHistoryRepository(NotificationDbContext dbContext, ICacheBroker cacheBroker)
        : base(dbContext, cacheBroker)
    {
    }

    public new async ValueTask<EmailHistory> CreateAsync(EmailHistory emailHistory, bool saveChanges, CancellationToken cancellationToken)
    {
        if(emailHistory.EmailTemplate is not null)
            DbContext.Entry(emailHistory.EmailTemplate).State = EntityState.Unchanged;

        var createdHistory = await base.CreateAsync(emailHistory, saveChanges, cancellationToken);

        if (createdHistory.EmailTemplate is not null)
            DbContext.Entry(emailHistory.EmailTemplate).State = EntityState.Detached;

        return createdHistory;
    }

    public new IQueryable<EmailHistory> Get(Expression<Func<EmailHistory, bool>>? predicate, bool asNoTracking)
    {
        return Get(predicate, asNoTracking);
    }
}
