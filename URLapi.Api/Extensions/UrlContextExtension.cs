using URLapi.Domain.IRepositories;
using URLapi.Domain.IServices;
using URLapi.Infra.Repositories;
using URLapi.Infra.Services;

namespace URLapi.Api.Extensions;

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