using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Queries.Urls;

public sealed record GetBigUrlByShortUrlQuery(string ShortUrl) : IQuery<IResult>
{
    public required string ShortUrl { get; init; } = ShortUrl;
}