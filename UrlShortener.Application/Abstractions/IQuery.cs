using Mediator;

namespace UrlShortener.Application.Abstractions;

public interface IQuery<out TResult> : IRequest<TResult>
{
}