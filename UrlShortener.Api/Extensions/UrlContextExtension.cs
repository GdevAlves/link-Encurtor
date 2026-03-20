using UrlShortener.Domain.IRepositories;
using UrlShortener.Domain.IServices;
using UrlShortener.Infra.Repositories;
using UrlShortener.Infra.Services;

namespace UrlShortener.Api.Extensions;

public static class UrlContextExtension
{
    public static void AddUrlContext(this WebApplicationBuilder builder)
    {
        // Repositories
        builder.Services.AddScoped<IUrlRepository, UrlRepository>();

        // Services
        builder.Services.AddScoped<IUrlService, UrlService>();
        builder.Services.AddScoped<IAccessService, AccessService>();
    }
}