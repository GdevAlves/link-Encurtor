using Microsoft.EntityFrameworkCore;
using URLapi.Domain.DomainDTOs;
using URLapi.Domain.Entities;
using URLapi.Domain.IRepositories;
using URLapi.Infra.Data;

namespace URLapi.Infra.Repositories;

public class UrlAccessRepository(AppDbContext context) : IUrlAccessRepository
{
    public Task SaveAsync(UrlAccessLog accessLog, CancellationToken cancellationToken)
    {
        context.UrlAccessLogs.Add(accessLog);
        return context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUrlAccessLogAsync(UrlAccessLog accessLog, CancellationToken cancellationToken)
    {
        context.UrlAccessLogs.Remove(accessLog);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TopUrlDto>> GetTopUrlsAsync(Guid userId, int daysAgo, int limit)
    {
        var startDate = DateTime.UtcNow.AddDays(-daysAgo);

        var query = await context.UrlAccessLogs
            .Where(log => log.Url.Creator.Id == userId && log.AccessedAt >= startDate)
            .GroupBy(log => new { log.UrlId, log.Url.ShortUrl, log.Url.LongUrl })
            .Select(g => new TopUrlDto
            {
                UrlId = g.Key.UrlId,
                ShortUrl = g.Key.ShortUrl,
                LongUrl = g.Key.LongUrl,
                AccessCount = g.Count(),
                LastAccessed = g.Max(l => l.AccessedAt)
            })
            .OrderByDescending(x => x.AccessCount)
            .Take(limit)
            .ToListAsync();

        return query;
    }

    public async Task<AccessPatternDto> GetAccessPatternAsync(Guid urlId, int daysAgo)
    {
        var startDate = DateTime.UtcNow.AddDays(-daysAgo);

        var hourlyPattern = await context.UrlAccessLogs
            .Where(log => log.UrlId == urlId && log.AccessedAt >= startDate)
            .GroupBy(log => log.AccessedAt.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderBy(x => x.Hour)
            .ToListAsync();

        var dailyPattern = await context.UrlAccessLogs
            .Where(log => log.UrlId == urlId && log.AccessedAt >= startDate)
            .GroupBy(log => log.AccessedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return new AccessPatternDto
        {
            HourlyDistribution = hourlyPattern.ToDictionary(x => x.Hour, x => x.Count),
            DailyTrend = dailyPattern.ToDictionary(x => x.Date, x => x.Count),
            PeakHour = hourlyPattern.OrderByDescending(x => x.Count).First().Hour,
            TotalAccesses = hourlyPattern.Sum(x => x.Count)
        };
    }

    // public Task<GeographicDistributionDto> GetGeographicDistributionAsync(Guid urlId, int daysAgo)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public async Task<List<TrendingUrlDto>> GetTrendingUrlsAsync(Guid userId, double growthThreshold)
    // {
    //     var lastWeekStart = DateTime.UtcNow.AddDays(-7);
    //     var previousWeekStart = DateTime.UtcNow.AddDays(-14);
    //     
    //     var lastWeekData = await context.UrlAccessLogs
    //         .Where(log => log.Url.Creator.Id == userId && log.AccessedAt >= lastWeekStart)
    //         .GroupBy(log => log.UrlId)
    //         .Select(g => new { UrlId = g.Key, Count = g.Count() })
    //         .ToListAsync();
    //     
    //     var previousWeekData = await context.UrlAccessLogs
    //         .Where(log => log.Url.Creator.Id == userId 
    //             && log.AccessedAt >= previousWeekStart 
    //             && log.AccessedAt < lastWeekStart)
    //         .GroupBy(log => log.UrlId)
    //         .Select(g => new { UrlId = g.Key, Count = g.Count() })
    //         .ToListAsync();
    //     
    //     var trending = new List<TrendingUrlDto>();
    //     
    //     foreach (var current in lastWeekData)
    //     {
    //         var previous = previousWeekData.FirstOrDefault(p => p.UrlId == current.UrlId);
    //         var previousCount = previous?.Count ?? 1;
    //         var growth = ((current.Count - previousCount) / (double)previousCount) * 100;
    //         
    //         if (growth >= growthThreshold)
    //         {
    //             var url = await context.Urls.FindAsync(current.UrlId);
    //             trending.Add(new TrendingUrlDto
    //             {
    //                 UrlId = current.UrlId,
    //                 ShortUrl = url.ShortUrl,
    //                 GrowthPercentage = growth,
    //                 CurrentWeekAccesses = current.Count,
    //                 PreviousWeekAccesses = previousCount
    //             });
    //         }
    //     }
    //     
    //     return trending.OrderByDescending(x => x.GrowthPercentage).ToList();
    // }
    //
    // public Task<PredictedTrafficDto> PredictTrafficAsync(Guid urlId, int daysToPredict)
    // {
    //     throw new NotImplementedException();
    // }
}