using Mediator;
using URLapi.Domain.Abstractions;

namespace URLapi.Domain.UseCases.Commands.Users;

public sealed record CreateUserCommand : IRequest<IResult>
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}