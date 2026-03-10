using Mediator;
using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Commands.Urls;

public sealed record DeleteUrlCommand : IRequest<IResult>
{
    public required Guid Id { get; init; }
}