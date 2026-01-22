using Lummo.Domain.Entities;
using Lummo.Persistence.Caching.Brokers.Interfaces;
using Lummo.Persistence.Caching.Models;
using Lummo.Persistence.DataContexts;
using Lummo.Persistence.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories;

public class StorageFileRepository(AppDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<StorageFile, AppDbContext>(dbContext, cacheBroker, new CacheEntryOptions())
    , IStorageFileRepository
{
    IQueryable<StorageFile> IStorageFileRepository.Get(Expression<Func<StorageFile, bool>>? predicate, bool asNoTracking)
    =>base.Get(predicate, asNoTracking);
}
