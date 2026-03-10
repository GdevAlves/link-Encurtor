using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Queries.Urls;

public class GetUrlInfoByShortUrlQuery(string shortUrl) : IQuery<IResult>
{
    public required string ShortUrl { get; init; } = shortUrl;
}