using Microsoft.EntityFrameworkCore;
using URLapi.Domain.Entities;
using URLapi.Infra.Mapping;

namespace URLapi.Infra.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Url> Urls { get; set; }
    public DbSet<UrlAccessLog> UrlAccessLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMap());
        modelBuilder.ApplyConfiguration(new UrlMap());
        modelBuilder.ApplyConfiguration(new UrlAccessLogMap());
    }
}