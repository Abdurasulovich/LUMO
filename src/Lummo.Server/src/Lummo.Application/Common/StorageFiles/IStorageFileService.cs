using Lummo.Domain.Entities;
using System.Linq.Expressions;

namespace Lummo.Application.Common.StorageFiles;

public interface IStorageFileService
{
    IQueryable<StorageFile> Get(Expression<Func<StorageFile, bool>>? predicate = default,
        bool asNoTracking = false);
}
