using URLapi.Domain.DomainDTOs;
using URLapi.Domain.Entities;

namespace URLapi.Domain.IRepositories;

public interface IUrlAccessRepository
{
    Task SaveAsync(UrlAccessLog accessLog, CancellationToken cancellationToken);
    Task DeleteUrlAccessLogAsync(UrlAccessLog url, CancellationToken cancellationToken);
    Task<List<TopUrlDto>> GetTopUrlsAsync(Guid userId, int daysAgo, int limit);
    
    Task<AccessPatternDto> GetAccessPatternAsync(Guid urlId, int daysAgo);
    // Task<GeographicDistributionDto> GetGeographicDistributionAsync(Guid urlId, int daysAgo);
    // Task<List<TrendingUrlDto>> GetTrendingUrlsAsync(Guid userId, double growthThreshold);
    // Task<PredictedTrafficDto> PredictTrafficAsync(Guid urlId, int daysToPredict);
}

