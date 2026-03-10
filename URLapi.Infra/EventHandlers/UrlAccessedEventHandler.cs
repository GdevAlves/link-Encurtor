using Mediator;
using URLapi.Application.Events;
using URLapi.Domain.IRepositories;

namespace URLapi.Infra.EventHandlers;

public class UrlAccessedEventHandler(IUrlRepository urlRepository) : INotificationHandler<UrlAccessedEvent>
{
    private readonly IUrlRepository _urlRepository = urlRepository;
    public async ValueTask Handle(UrlAccessedEvent notification, CancellationToken cancellationToken)
    {
        var url = await _urlRepository.GetUrlByIdAsync(notification.UrlId, cancellationToken);
        if (url == null)
            return;
        url.IncrementAccessCount();
        await _urlRepository.UpdateAsync(url, cancellationToken);
    }
}