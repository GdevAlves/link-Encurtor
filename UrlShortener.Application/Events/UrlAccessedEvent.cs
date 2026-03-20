using Mediator;

namespace UrlShortener.Application.Events;

public class UrlAccessedEvent(Guid urlId) : INotification
{
    public Guid UrlId { get; } = urlId;
}