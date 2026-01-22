using MediatR;

namespace Lummo.Domain.Commands.Interfaces;
public interface ICommand<out TResult> : ICommand, IRequest<TResult>{}

public interface ICommand {}