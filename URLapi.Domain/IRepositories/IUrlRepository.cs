using URLapi.Domain.Entities;

namespace URLapi.Domain.IRepositories;

public interface IUrlRepository
{
    Task<Url?> GetUrlByShortUrlAsync(string shortUrl, CancellationToken cancellationToken);
    Task SaveAsync(Url url, CancellationToken cancellationToken);
    Task<Url?> GetUrlByIdAsync(Guid urlId, CancellationToken cancellationToken);
    Task DeleteUrlAsync(Url url, CancellationToken cancellationToken);
    Task<Url?> GetUrlByShortUrlWithCreatorAsync(string shortUrl, CancellationToken cancellationToken);
}