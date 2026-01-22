using MediatR;

namespace Lummo.Domain.Commands.Interfaces;

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
}