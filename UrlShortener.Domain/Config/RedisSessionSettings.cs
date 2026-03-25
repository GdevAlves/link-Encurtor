namespace UrlShortener.Domain.Config;

public class RedisSessionSettings
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; init; } = "localhost:6379";
    public int SessionTtlMinutes { get; init; } = 120;
}

