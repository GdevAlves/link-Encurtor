using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.UseCases.Queries.Users;

public sealed record GetUserByIdQuery : IQuery<IResult>
{
    public required Guid UserId { get; init; }
}