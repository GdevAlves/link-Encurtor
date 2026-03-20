using Microsoft.Extensions.Options;
using URLapi.Application.AI;
using UrlShortener.Domain.Config;
using UrlShortener.Domain.IRepositories;
using UrlShortener.Infra.Repositories;

namespace UrlShortener.Api.Extensions;

public static class AiContextExtension
{
    public static void AddAiContext(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection(GeminiSettings.SectionName));

        // Repository needed by analytics plugin/tools
        builder.Services.AddScoped<IUrlAccessRepository, UrlAccessRepository>();

        // Factory used by query handlers to create the agent
        builder.Services.AddScoped<AgentFactory>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<GeminiSettings>>().Value;
            if (string.IsNullOrWhiteSpace(settings.ApiKey))
                throw new InvalidOperationException(
                    "Gemini ApiKey não configurada. Defina via User Secrets: 'Gemini:ApiKey' ou em appsettings.json.");
            return new AgentFactory(settings.ApiKey, sp);
        });
    }
}