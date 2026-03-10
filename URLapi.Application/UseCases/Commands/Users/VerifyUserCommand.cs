using Mediator;
using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Commands.Users;

public class VerifyUserCommand : IRequest<IResult>
{
    public Guid Token { get; init; }
}