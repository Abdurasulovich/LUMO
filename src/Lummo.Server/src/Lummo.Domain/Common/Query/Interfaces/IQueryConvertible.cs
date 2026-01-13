using Lummo.Domain.Common.Entities;

namespace Lummo.Domain.Common.Query.Interfaces;

public interface IQueryConvertible<TEntity> where TEntity : Entity
{
    QuerySpecification<TEntity> ToQuerySpecification();
}

public interface IQueryConvertible
{
    QuerySpecification ToQuerySpecification();
}
