using Mediator;
using URLapi.Domain.Abstractions;

namespace URLapi.Domain.UseCases.Commands.Users;

public class VerifyUserCommand : IRequest<IResult>
{
    public Guid Token { get; init; }
}