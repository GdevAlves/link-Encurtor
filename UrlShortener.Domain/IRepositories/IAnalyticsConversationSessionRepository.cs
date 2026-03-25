namespace UrlShortener.Domain.IRepositories;

public interface IAnalyticsConversationSessionRepository
{
    Task<string?> GetSessionAsync(Guid userId, Guid conversationId, CancellationToken cancellationToken);
    Task SaveSessionAsync(Guid userId, Guid conversationId, string serializedSession, CancellationToken cancellationToken);
    Task DeleteSessionAsync(Guid userId, Guid conversationId, CancellationToken cancellationToken);
}

