using Flunt.Notifications;
using Flunt.Validations;
using Mediator;
using UrlShortener.Application.DTOs.UrlDTO;
using UrlShortener.Application.DTOs.UtilsDTO;
using UrlShortener.Application.Enums;
using UrlShortener.Application.Events;
using UrlShortener.Application.UseCases.Commands;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.IRepositories;
using UrlShortener.Domain.IServices;
using Abstractions_IResult = UrlShortener.Application.Abstractions.IResult;
using IResult = UrlShortener.Application.Abstractions.IResult;

namespace UrlShortener.Application.UseCases.Queries.Urls;

public class UrlQueryHandler(
    IUrlRepository urlRepository,
    IUrlAccessRepository urlAccessRepository,
    IAccessService accessService,
    ICurrentUserService currentUserService,
    IMediator mediator
)
    : Abstractions.IQueryHandler<GetLongUrlByShortUrlQuery, Abstractions_IResult>,
        Abstractions.IQueryHandler<GetUrlInfoByShortUrlQuery, Abstractions_IResult>,
        Abstractions.IQueryHandler<GetUsersUrlsByUserIdQuery, Abstractions_IResult>
{
    public async ValueTask<Abstractions_IResult> Handle(GetLongUrlByShortUrlQuery query, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotEmpty(query.ShortUrl, "ShortUrl", "A URL é obrigatória.");

        if (!contract.IsValid)
            return new Result(ResultStatus.ValidationError, false, "Validação falhou",
                contract.Notifications);

        var shortUrl = query.ShortUrl;
        var bigUrl = await urlRepository.GetUrlByShortUrlAsync(shortUrl, cancellationToken);

        if (bigUrl == null)
            return new Result(ResultStatus.NotFound, false, "URL not found.");

        await mediator.Publish(new UrlAccessedEvent(bigUrl.Id), cancellationToken);

        var access = new UrlAccessLog
        {
            UrlId = bigUrl.Id,
            AccessedAt = DateTime.UtcNow,
            UserAgent = accessService.GetUserAgent(),
            IpAddress = accessService.GetIpAddress(),
            Referer = accessService.GetReferer(),
            Country = "", // TODO 
            City = ""
        };

        await urlAccessRepository.SaveAsync(access, cancellationToken);
        return new Result(ResultStatus.Success, true, "success", bigUrl.LongUrl);
    }

    public async ValueTask<Abstractions_IResult> Handle(GetUrlInfoByShortUrlQuery query, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotEmpty(query.ShortUrl, "ShortUrl", "A URL é obrigatória.");
        if (!contract.IsValid)
            return new Result(ResultStatus.ValidationError, false, "Validação falhou",
                contract.Notifications);

        var userId = currentUserService.GetUserId();

        var shortUrl = query.ShortUrl;

        var bigUrl = await urlRepository.GetUrlByShortUrlWithCreatorAsync(shortUrl, cancellationToken);

        if (bigUrl == null) return new Result(ResultStatus.NotFound, false, "URL not found.");

        if (bigUrl.Creator.Id != userId)
            return new Result(ResultStatus.Forbidden, false,
                "Você não está autorizado a acessar esta URL.");

        var urlDto = new UrlDTO
        {
            Id = bigUrl.Id,
            ShortUrl = bigUrl.ShortUrl,
            LongUrl = bigUrl.LongUrl,
            AccessCount = bigUrl.AccessCount
        };
        return new Result(ResultStatus.Success, true, "success", urlDto);
    }

    public async ValueTask<Abstractions_IResult> Handle(GetUsersUrlsByUserIdQuery request, CancellationToken cancellationToken)
    { 
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotEmpty(request.Id, "UserId", "O ID do usuário é obrigatório.")
            .IsGreaterThan(request.Page, 0, "Page", "A página deve ser maior que zero.")
            .IsGreaterThan(request.PageSize, 0, "PageSize", "O tamanho da página deve ser maior que zero.");
        if (!contract.IsValid)
            return new Result(ResultStatus.ValidationError, false, "Validação falhou",
                contract.Notifications);
        var userId = currentUserService.GetUserId();

        if (userId != request.Id)
            return new Result(ResultStatus.Forbidden, false,
                "Você não está autorizado a acessar as URLs deste usuário.");

        var page = request.Page;
        var pageSize = request.PageSize;

        var (userUrls, totalCount) = await urlRepository.GetUrlsByUserIdAsync(userId, page, pageSize, cancellationToken);

        if (userUrls is null || (!userUrls.Any()))
            return new Result(ResultStatus.NotFound, false, "Nenhuma URL encontrada para este usuário.");

        var urlDtos = userUrls.Select(url => new UrlDTO
        {
            Id = url.Id,
            LongUrl = url.LongUrl,
            ShortUrl = url.ShortUrl,
            AccessCount = url.AccessCount
        }).ToArray();

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var pagination = new PaginationDto<IEnumerable<UrlDTO>>
        {
            Data = urlDtos,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalCount = totalCount
        };

        return new Result(ResultStatus.Success, true, "success", pagination);
    }
}