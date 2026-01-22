using Lummo.Application.Common.StorageFiles;
using Lummo.Domain.Entities;
using Lummo.Persistence.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Lummo.Infrastructure.Common.StorageFiles;

public class StorageFileService(IStorageFileRepository storageFileRepository) : IStorageFileService
{
    public IQueryable<StorageFile> Get(Expression<Func<StorageFile, bool>>? predicate = null, bool asNoTracking = false)
        => storageFileRepository.Get(predicate, asNoTracking);
}
