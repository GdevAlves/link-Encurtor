using Mediator;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.UseCases.Commands.Urls;

public sealed record DeleteUrlCommand : IRequest<IResult>
{
    public required Guid Id { get; init; }
}