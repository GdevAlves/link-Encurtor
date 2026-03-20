using Mediator;
using UrlShortener.Application.Events;
using UrlShortener.Domain.IRepositories;

namespace UrlShortener.Infra.EventHandlers;

public class UrlAccessedEventHandler(IUrlRepository urlRepository) : INotificationHandler<UrlAccessedEvent>
{
    public async ValueTask Handle(UrlAccessedEvent notification, CancellationToken cancellationToken)
    {
        var url = await urlRepository.GetUrlByIdAsync(notification.UrlId, cancellationToken);
        if (url == null)
            return;
        url.IncrementAccessCount();
        await urlRepository.UpdateAsync(url, cancellationToken);
    }
}