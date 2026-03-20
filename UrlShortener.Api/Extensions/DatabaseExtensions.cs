using Microsoft.EntityFrameworkCore;
using UrlShortener.Infra.Data;

namespace UrlShortener.Api.Extensions;

public static class DatabaseExtensions
{
    public static WebApplication EnsureDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        return app;
    }
}