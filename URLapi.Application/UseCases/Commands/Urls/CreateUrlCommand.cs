using Mediator;
using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Commands.Urls;

public sealed record CreateUrlCommand : IRequest<IResult>
{
    public required string BigUrl { get; init; }
    public string? WantedShortUrl { get; init; }
}