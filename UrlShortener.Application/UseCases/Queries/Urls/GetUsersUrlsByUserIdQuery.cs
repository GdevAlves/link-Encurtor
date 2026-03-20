using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.UseCases.Queries.Urls;

public class GetUsersUrlsByUserIdQuery(Guid id, int page = 1, int pageSize = 10) : IQuery<IResult>
{
    public Guid Id { get; set; } = id;
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
}