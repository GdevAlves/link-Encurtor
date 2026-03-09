using Mediator;

namespace URLapi.Domain.Abstractions;

public interface IQuery<out TResult> : IRequest<TResult>
{
}