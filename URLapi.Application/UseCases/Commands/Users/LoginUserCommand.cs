using Mediator;
using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Commands.Users;

public sealed record LoginUserCommand : IRequest<IResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}