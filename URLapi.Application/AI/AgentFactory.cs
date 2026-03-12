using Microsoft.Extensions.DependencyInjection;
using URLapi.Application.Agents;
using URLapi.Domain.IRepositories;

namespace URLapi.Application.AI;

public class AgentFactory(string apiKey, IServiceProvider serviceProvider)
{
    public UrlAnalyticsAgent CreateUrlAnalyticsAgent()
    {
        var analyticsService = serviceProvider.GetRequiredService<IUrlAccessRepository>();
        return new UrlAnalyticsAgent(analyticsService, apiKey);
    }

}