using Microsoft.EntityFrameworkCore;
using URLapi.Domain.Entities;
using URLapi.Domain.IRepositories;
using URLapi.Infra.Data;

namespace URLapi.Infra.Repositories;

public class UrlRepository(AppDbContext context) : IUrlRepository
{
    public async Task<Url?> GetUrlByShortUrlAsync(string shortUrl, CancellationToken cancellationToken)
    {
        return await context.Urls.AsNoTracking().FirstOrDefaultAsync(x =>
            x.ShortUrl == shortUrl, cancellationToken);
    }

    public async Task SaveAsync(Url url, CancellationToken cancellationToken)
    {
        await context.Urls.AddAsync(url, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Url?> GetUrlByIdAsync(Guid urlId, CancellationToken cancellationToken)
    {
        return await context.Urls.FirstOrDefaultAsync(x => x.Id == urlId, cancellationToken);
    }

    public async Task DeleteUrlAsync(Url url, CancellationToken cancellationToken)
    {
        context.Urls.Remove(url);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Url?> GetUrlByShortUrlWithCreatorAsync(string shortUrl, CancellationToken cancellationToken)
    {
        return await context.Urls
            .Include(x => x.Creator)
            .FirstOrDefaultAsync(x => x.ShortUrl == shortUrl, cancellationToken);
    }

    public async Task UpdateAsync(Url url, CancellationToken cancellationToken)
    {
        context.Urls.Update(url);
        await context.SaveChangesAsync(cancellationToken);
    }
}