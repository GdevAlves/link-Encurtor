using URLapi.Domain.Abstractions;

namespace URLapi.Domain.UseCases.Queries.Urls;

public class GetUrlInfoByShortUrlQuery(string shortUrl) : IQuery<IResult>
{
    public required string ShortUrl { get; init; } = shortUrl;
}