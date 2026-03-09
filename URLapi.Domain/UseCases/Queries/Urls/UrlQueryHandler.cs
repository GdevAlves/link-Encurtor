using System.Net;
using Flunt.Notifications;
using Flunt.Validations;
using URLapi.Domain.Abstractions;
using URLapi.Domain.DTOs.UrlDTO;
using URLapi.Domain.IRepositories;
using URLapi.Domain.IServices;
using URLapi.Domain.UseCases.Commands;

namespace URLapi.Domain.UseCases.Queries.Urls;

public class UrlQueryHandler(IUrlRepository urlRepository, ICurrentUserService currentUserService)
    : IQueryHandler<GetBigUrlByShortUrlQuery, IResult>,
        IQueryHandler<GetUrlInfoByShortUrlQuery, IResult>
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

        return bigUrl is null
            ? new Result(HttpStatusCode.NotFound, false, "URL not found.")
            : new Result(HttpStatusCode.OK, true, "success", bigUrl.LongUrl);
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
            LongUrl = bigUrl.LongUrl
        };
        return new Result(HttpStatusCode.OK, true, "success", urlDto);
    }
}