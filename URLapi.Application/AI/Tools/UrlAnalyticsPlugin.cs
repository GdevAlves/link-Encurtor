using System.Text.Json;
using URLapi.Domain.IRepositories;

namespace URLapi.Application.AI.Tools;

public class UrlAnalyticsPlugin(IUrlAccessRepository urlAccessRepository)
{
    public async Task<string> GetTopUrls(Guid userId, int daysAgo = 7, int limit = 10)
    {
        var data = await urlAccessRepository.GetTopUrlsAsync(userId, daysAgo, limit);
        return JsonSerializer.Serialize(data);
    }

    public async Task<string> GetAccessPattern(Guid urlId, int daysAgo = 14)
    {
        var pattern = await urlAccessRepository.GetAccessPatternAsync(urlId, daysAgo);
        return JsonSerializer.Serialize(pattern);
    }

    // public async Task<string> GetGeographicDistribution(Guid urlId, int daysAgo = 30)
    // {
    //     var distribution = await urlAccessRepository.GetGeographicDistributionAsync(urlId, daysAgo);
    //     return JsonSerializer.Serialize(distribution);
    // }
    //
    // public async Task<string> GetTrendingUrls(Guid userId, double growthThreshold = 50.0)
    // {
    //     var trending = await urlAccessRepository.GetTrendingUrlsAsync(userId, growthThreshold);
    //     return JsonSerializer.Serialize(trending);
    // }
}