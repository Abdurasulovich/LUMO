using Lummo.Domain.Common.Caching;
using Lummo.Domain.Comparers;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Formats.Asn1;
using System.Linq.Expressions;

namespace Lummo.Domain.Common.Query;

public class QuerySpecification<TSource>(uint pageSize, uint pageToken, bool asNoTracking, int? filterHashCode = default) : ICacheModel
{

    public List<Expression<Func<TSource, bool>>> FilteringOptions { get; } = [];

    public List<(Expression<Func<TSource, object>> KeySelector, bool IsAscending)> OrderingOptions { get; } = [];

    public List<Expression<Func<TSource, object>>> IncludingOptions { get; } = [];

    public FilterPagination PaginationOptions { get; } = new()
    {
        PageSize = pageSize,
        PageToken = pageToken
    };

    public bool AsNoTracking { get; } = asNoTracking;

    public int? FilterHashCode { get; } = filterHashCode;

    public string CacheKey => $"{typeof(TSource).Name}_{GetHashCode()}";

    public override int GetHashCode()
    {
        if (FilterHashCode is not null) return FilterHashCode.Value;

        var hashCode = new HashCode();
        var expressionEqualityComparer = ExpressionEqualityComparer.Instance;

        foreach (var filter in FilteringOptions.Order(new PredicateExpressionComparer<TSource>()))
            hashCode.Add(expressionEqualityComparer.GetHashCode(filter));

        foreach (var include in IncludingOptions.Order(new KeySelectorExpressionComparer<TSource>()))
            hashCode.Add(expressionEqualityComparer.GetHashCode(include));
        foreach (var order in OrderingOptions)
            hashCode.Add(expressionEqualityComparer.GetHashCode(order.KeySelector));

        hashCode.Add(PaginationOptions);

        return hashCode.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is QuerySpecification<TSource> querySpecification && querySpecification.GetHashCode() == GetHashCode();
    }
}

public class QuerySpecification : ICacheModel
{
    public FilterPagination PaginationOptions { get; set; }

    public bool AsNoTracking { get; }

    public QuerySpecification(uint pageSize, uint pageToken, bool asNoTracking)
    {
        PaginationOptions = new FilterPagination(pageSize, pageToken);
        AsNoTracking = asNoTracking;
    }

    public QuerySpecification(FilterPagination filterPagination, bool asNoTracking)
    {
        PaginationOptions = filterPagination;
        AsNoTracking = asNoTracking;
        
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(PaginationOptions);
        return hashCode.ToHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is QuerySpecification querySpecification && querySpecification.GetHashCode() == GetHashCode();
    }
    public string CacheKey => GetHashCode().ToString();
}
