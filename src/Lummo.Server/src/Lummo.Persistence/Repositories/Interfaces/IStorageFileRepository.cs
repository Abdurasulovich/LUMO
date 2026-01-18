using Lummo.Domain.Entities;
using System.Linq.Expressions;

namespace Lummo.Persistence.Repositories.Interfaces;

public interface IStorageFileRepository
{
    IQueryable<StorageFile> Get(Expression<Func<StorageFile, bool>>? predicate = default,
        bool asNoTracking = false);
}
