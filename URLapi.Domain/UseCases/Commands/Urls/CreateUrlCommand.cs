using Mediator;
using URLapi.Domain.Abstractions;

namespace URLapi.Domain.UseCases.Commands.Urls;

public sealed record CreateUrlCommand : IRequest<IResult>
{
    public required string BigUrl { get; init; }
    public string? WantedShortUrl { get; init; }
}