using Mediator;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.UseCases.Commands.Users;

public class VerifyUserCommand : IRequest<IResult>
{
    public Guid Token { get; init; }
}