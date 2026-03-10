using Mediator;

namespace URLapi.Application.Abstractions;

public interface IQuery<out TResult> : IRequest<TResult>
{
}