using Mediator;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.UseCases.Commands.Users;

public sealed record DeleteUserCommand : IRequest<IResult>
{
    public required Guid UserId { get; init; }
}