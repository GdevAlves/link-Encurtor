using Mediator;
using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Commands.Users;

public sealed record DeleteUserCommand : IRequest<IResult>
{
    public required Guid UserId { get; init; }
}