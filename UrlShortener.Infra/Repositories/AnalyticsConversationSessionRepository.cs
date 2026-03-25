using StackExchange.Redis;
using UrlShortener.Domain.Config;
using UrlShortener.Domain.IRepositories;

namespace UrlShortener.Infra.Repositories;

public sealed class AnalyticsConversationSessionRepository(
    IConnectionMultiplexer redis,
    RedisSessionSettings settings) : IAnalyticsConversationSessionRepository
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(Math.Max(1, settings.SessionTtlMinutes));

    public async Task<string?> GetSessionAsync(Guid userId, Guid conversationId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(userId, conversationId);
        var value = await _database.StringGetAsync(key);

        return value.HasValue ? value.ToString() : null;
    }

    public async Task SaveSessionAsync(Guid userId, Guid conversationId, string serializedSession,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(userId, conversationId);
        await _database.StringSetAsync(key, serializedSession, _ttl);
    }

    public async Task DeleteSessionAsync(Guid userId, Guid conversationId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(userId, conversationId);
        await _database.KeyDeleteAsync(key);
    }

    private static string BuildKey(Guid userId, Guid conversationId)
        => $"analytics:session:{userId:N}:{conversationId:N}";
}

