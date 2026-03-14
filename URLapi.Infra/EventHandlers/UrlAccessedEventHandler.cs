using Mediator;
using URLapi.Application.Events;
using URLapi.Domain.IRepositories;

namespace URLapi.Infra.EventHandlers;

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