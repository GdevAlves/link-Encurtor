using Mediator;
using URLapi.Domain.Abstractions;

namespace URLapi.Domain.UseCases.Commands.Users;

public sealed record DeleteUserCommand : IRequest<IResult>
{
    public required Guid UserId { get; init; }
}