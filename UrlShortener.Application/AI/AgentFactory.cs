using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Agents;
using UrlShortener.Domain.IRepositories;

namespace UrlShortener.Application.AI;

public class AgentFactory(string apiKey, IServiceProvider serviceProvider)
{
    public UrlAnalyticsAgent CreateUrlAnalyticsAgent()
    {
        var analyticsService = serviceProvider.GetRequiredService<IUrlAccessRepository>();
        return new UrlAnalyticsAgent(analyticsService, apiKey);
    }
}