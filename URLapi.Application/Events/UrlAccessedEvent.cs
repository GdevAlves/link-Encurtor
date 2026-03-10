using Mediator;

namespace URLapi.Application.Events;

public class UrlAccessedEvent(Guid urlId) : INotification
{
    public Guid UrlId { get; } = urlId;
}