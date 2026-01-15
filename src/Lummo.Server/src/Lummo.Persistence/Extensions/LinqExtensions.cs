using Lummo.Domain.Common.Entities.Interfaces;
using Lummo.Domain.Common.Query;
using Microsoft.EntityFrameworkCore;

namespace Lummo.Persistence.Extensions;

public static class LinqExtensions
{
    public static IQueryable<TSource> ApplySpecification<TSource>(this IQueryable<TSource> sources,
        QuerySpecification<TSource> querySpecification)  where TSource :class, IEntity
    {
        sources = sources
            .ApplyPagination(querySpecification)
            .ApplyPredicates(querySpecification)
            .ApplyOrdering(querySpecification)
            .ApplyIncluding(querySpecification);
        return sources;
    }

    public static IEnumerable<TSource> ApplySpecification<TSource>(this IEnumerable<TSource> sources, 
        QuerySpecification<TSource> querySpecification) where TSource : IEntity
    {
        sources = sources
            .ApplyPagination(querySpecification)
            .ApplyPredicates(querySpecification)
            .ApplyOrdering(querySpecification);

        return sources;
    }

    public static IQueryable<TSource> ApplySpecification<TSource>(this IQueryable<TSource> sources,
        QuerySpecification querySpecification) where TSource : class, IEntity
    {
        sources = sources
            .ApplyPagination(querySpecification);

        return sources;
    }

    public static IEnumerable<TSource> ApplySpecification<TSource>( this IEnumerable<TSource> sources,
        QuerySpecification querySpecification) where TSource : IEntity
    {
        sources = sources
            .ApplyPagination(querySpecification);
        return sources;
    }

    public static IQueryable<TSource> ApplyPredicates<TSource>(this IQueryable<TSource> sources,
        QuerySpecification<TSource> querySpecification) where TSource : IEntity
    {
        querySpecification.FilteringOptions.ForEach(predicate=> sources = sources.Where(predicate));

        return sources;
    }

    public static IEnumerable<TSource> ApplyPredicates<TSource>(this IEnumerable<TSource> sources,
        QuerySpecification<TSource> querySpecification) where TSource : IEntity
    {
        querySpecification.FilteringOptions.ForEach(predicate => sources = sources.Where(predicate.Compile()));
        return sources;
    }

    public static IQueryable<TSource> ApplyOrdering<TSource>(this IQueryable<TSource> sources,
        QuerySpecification<TSource> querySpecification) where TSource : IEntity
    {
        if(!querySpecification.OrderingOptions.Any())
            sources.OrderBy(entity=> entity.Id);

        querySpecification.OrderingOptions.ForEach(
            orderingExpression => sources = orderingExpression.IsAscending
            ? sources.OrderBy(orderingExpression.Item1)
            : sources.OrderByDescending(orderingExpression.Item1)
            );

        return sources;
    }

    public static IQueryable<TSource> ApplyIncluding<TSource>(this IQueryable<TSource> sources,
        QuerySpecification<TSource> querySpecification) where TSource : class, IEntity
    {
        querySpecification.IncludingOptions.ForEach(
            includingOption => sources = sources.Include(includingOption)
            );
        return sources;
    }

    public static IEnumerable<TSource> ApplyOrdering<TSource>(this IEnumerable<TSource> sources,
        QuerySpecification<TSource> querySpecification) where TSource : IEntity
    {
        if(querySpecification.OrderingOptions.Count == 0)
            return sources.OrderBy(entity=> entity.Id);

        querySpecification.OrderingOptions.ForEach(
            orderingExpression => sources = orderingExpression.IsAscending
            ? sources.OrderBy(orderingExpression.Item1.Compile())
            : sources.OrderByDescending(orderingExpression.Item1.Compile())
            );

        return sources;
    }

    public static IQueryable<TSource> ApplyPagination<TSource>(this IQueryable<TSource> sources,
        QuerySpecification<TSource> querySpecification) where TSource : IEntity
    {
        return sources.Skip((int)((querySpecification.PaginationOptions.PageToken - 1) *
            querySpecification.PaginationOptions.PageSize))
            .Take((int)querySpecification.PaginationOptions.PageSize);
    }

    public static IEnumerable<TSource> ApplyPagination<TSource>(this IEnumerable<TSource> sources,
        QuerySpecification<TSource> querySpecification) where TSource : IEntity
    {
        return sources.Skip((int)((querySpecification.PaginationOptions.PageToken - 1) *
            querySpecification.PaginationOptions.PageSize))
            .Take((int)querySpecification.PaginationOptions.PageSize);
    }

    public static IQueryable<TSource> ApplyPagination<TSource>(this IQueryable<TSource> sources,
        QuerySpecification querySpecification) where TSource : IEntity
    {
        return sources.Skip((int)((querySpecification.PaginationOptions.PageToken - 1) *
            querySpecification.PaginationOptions.PageSize))
            .Take((int)querySpecification.PaginationOptions.PageSize);
    }

    public static IEnumerable<TSource> ApplyPagination<TSource>(this IEnumerable<TSource> sources,
        QuerySpecification querySpecification) where TSource : IEntity
    {
        return sources.Skip((int)((querySpecification.PaginationOptions.PageToken - 1) *
            querySpecification.PaginationOptions.PageSize))
            .Take((int)querySpecification.PaginationOptions.PageSize);
    }

    public static IQueryable<TSource> ApplyPagination<TSource>(this IQueryable<TSource> sources,
        FilterPagination paginationOptions)
    {
        return sources.Skip((int)((paginationOptions.PageToken - 1) *
            paginationOptions.PageSize))
            .Take((int)paginationOptions.PageSize);
    }
}
