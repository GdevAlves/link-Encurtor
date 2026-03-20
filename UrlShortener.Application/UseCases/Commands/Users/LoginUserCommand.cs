using Mediator;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.UseCases.Commands.Users;

public sealed record LoginUserCommand : IRequest<IResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}