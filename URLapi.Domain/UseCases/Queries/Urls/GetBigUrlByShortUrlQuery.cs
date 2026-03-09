using URLapi.Domain.Abstractions;

namespace URLapi.Domain.UseCases.Queries.Urls;

public sealed record GetBigUrlByShortUrlQuery(string ShortUrl) : IQuery<IResult>
{
    public required string ShortUrl { get; init; } = ShortUrl;
}