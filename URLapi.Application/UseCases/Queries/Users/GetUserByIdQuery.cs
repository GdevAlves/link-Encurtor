using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Queries.Users;

public sealed record GetUserByIdQuery : IQuery<IResult>
{
    public required Guid UserId { get; init; }
}