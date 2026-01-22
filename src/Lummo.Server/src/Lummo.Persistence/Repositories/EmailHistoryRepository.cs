using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.Caching.Models;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories;

public class EmailHistoryRepository(AppDbContext dbContext, ICacheBroker cacheBroker) 
    : EntityRepositoryBase<EmailHistory, AppDbContext>(dbContext, cacheBroker, new CacheEntryOptions())
    , IEmailHistoryRepository
{
    public new ValueTask<EmailHistory> CreateAsync(EmailHistory emailHistory, bool saveChanges, 
        CancellationToken cancellationToken)
    => base.CreateAsync(emailHistory, saveChanges, cancellationToken);

    public new IQueryable<EmailHistory> Get(Expression<Func<EmailHistory, bool>>? predicate, bool asNoTracking)
    => base.Get(predicate, asNoTracking);
}
