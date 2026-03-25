using Mediator;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.UseCases.Commands.Urls;

public sealed record CreateUrlCommand : IRequest<IResult>
{
    public required string LongUrl { get; init; }
    public string? WantedShortUrl { get; init; }
}