using Mediator;
using URLapi.Domain.Abstractions;

namespace URLapi.Domain.UseCases.Commands.Urls;

public sealed record DeleteUrlCommand : IRequest<IResult>
{
    public required Guid Id { get; init; }
}