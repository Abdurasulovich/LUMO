using MediatR;

namespace Lummo.Domain.Queries.Interfaces;

public interface IQuery<out TResult> : IRequest<TResult>{}