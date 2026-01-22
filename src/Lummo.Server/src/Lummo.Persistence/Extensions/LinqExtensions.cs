using Lummo.Domain.Common.Query;

namespace Lummo.Persistence.Extensions;

public static class LinqExtensions
{
    public static IQueryable<TSource> ApplySpecification<TSource>(this IQueryable<TSource> sources,
        FilterPagination paginationOptions) 
    {
        return sources.Skip((int)((paginationOptions.PageToken -1) * paginationOptions.PageSize))
            .Take((int)paginationOptions.PageSize);
    }

    public static async Task<T?> FirstOrDefaultAsync<T>(
        this IEnumerable<T> source,
        Func<T, Task<bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        if (cancellationToken.IsCancellationRequested)
            return default;

        foreach (var item in source)
            if (await predicate(item))
                return item;

        return default;
    }
}
