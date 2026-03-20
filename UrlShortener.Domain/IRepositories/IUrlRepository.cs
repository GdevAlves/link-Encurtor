using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.IRepositories;

public interface IUrlRepository
{
    Task<Url?> GetUrlByShortUrlAsync(string shortUrl, CancellationToken cancellationToken);
    Task SaveAsync(Url url, CancellationToken cancellationToken);
    Task<Url?> GetUrlByIdAsync(Guid urlId, CancellationToken cancellationToken);
    Task DeleteUrlAsync(Url url, CancellationToken cancellationToken);
    Task<Url?> GetUrlByShortUrlWithCreatorAsync(string shortUrl, CancellationToken cancellationToken);
    Task UpdateAsync(Url url, CancellationToken cancellationToken);
    Task<(Url[] Items, int TotalCount)> GetUrlsByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken);
}