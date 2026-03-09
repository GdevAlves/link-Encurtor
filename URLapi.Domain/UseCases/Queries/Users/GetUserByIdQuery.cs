using URLapi.Domain.Abstractions;

namespace URLapi.Domain.UseCases.Queries.Users;

public sealed record GetUserByIdQuery : IQuery<IResult>
{
    public required Guid UserId { get; init; }
}