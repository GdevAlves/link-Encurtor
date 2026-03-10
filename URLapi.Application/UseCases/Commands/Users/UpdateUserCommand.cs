using Mediator;
using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Commands.Users;

public sealed record UpdateUserCommand : IRequest<IResult>
{
    public required Guid UserId { get; set; }
    public string? Name { get; init; }
    public string? Email { get; init; }
}