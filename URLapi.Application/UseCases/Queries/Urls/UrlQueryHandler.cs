using System.Net;
using Flunt.Notifications;
using Flunt.Validations;
using Mediator;
using URLapi.Application.DTOs.UrlDTO;
using URLapi.Application.Events;
using URLapi.Application.UseCases.Commands;
using URLapi.Domain.IRepositories;
using URLapi.Domain.IServices;
using IResult = URLapi.Application.Abstractions.IResult;

namespace URLapi.Application.UseCases.Queries.Urls;

public class UrlQueryHandler(
    IUrlRepository urlRepository,
    ICurrentUserService currentUserService,
    IMediator mediator
)
    : Abstractions.IQueryHandler<GetBigUrlByShortUrlQuery, IResult>, Abstractions.IQueryHandler<GetUrlInfoByShortUrlQuery, IResult>
{
    public async ValueTask<IResult> Handle(GetBigUrlByShortUrlQuery query, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotEmpty(query.ShortUrl, "ShortUrl", "A URL é obrigatória.");

        if (!contract.IsValid)
            return new Result(HttpStatusCode.BadRequest, false, "Validação falhou",
                contract.Notifications);

        var shortUrl = query.ShortUrl;
        var bigUrl = await urlRepository.GetUrlByShortUrlAsync(shortUrl, cancellationToken);

        if (bigUrl == null)
            return new Result(HttpStatusCode.NotFound, false, "URL not found.");

        await mediator.Publish(new UrlAccessedEvent(bigUrl.Id), cancellationToken);

        return new Result(HttpStatusCode.OK, true, "success", bigUrl.LongUrl);
    }

    public async ValueTask<IResult> Handle(GetUrlInfoByShortUrlQuery query, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotEmpty(query.ShortUrl, "ShortUrl", "A URL é obrigatória.");
        if (!contract.IsValid)
            return new Result(HttpStatusCode.BadRequest, false, "Validação falhou",
                contract.Notifications);

        var userId = currentUserService.GetUserId();

        var shortUrl = query.ShortUrl;

        var bigUrl = await urlRepository.GetUrlByShortUrlWithCreatorAsync(shortUrl, cancellationToken);

        if (bigUrl == null) return new Result(HttpStatusCode.NotFound, false, "URL not found.");

        if (bigUrl.Creator.Id != userId)
            return new Result(HttpStatusCode.Forbidden, false,
                "Você não está autorizado a acessar esta URL.");

        var urlDto = new UrlDTO
        {
            Id = bigUrl.Id,
            ShortUrl = bigUrl.ShortUrl,
            LongUrl = bigUrl.LongUrl,
            AccessCount = bigUrl.AccessCount,
        };
        return new Result(HttpStatusCode.OK, true, "success", urlDto);
    }
}