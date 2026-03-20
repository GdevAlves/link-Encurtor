using Mediator;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.UseCases.Commands.Users;

public sealed record UpdateUserCommand : IRequest<IResult>
{
    public required Guid UserId { get; set; }
    public string? Name { get; init; }
    public string? Email { get; init; }
}