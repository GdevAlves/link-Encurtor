using Flunt.Notifications;
using Flunt.Validations;
using Mediator;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.UrlDTO;
using UrlShortener.Application.Enums;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.IRepositories;
using UrlShortener.Domain.IServices;

namespace UrlShortener.Application.UseCases.Commands.Urls;

public sealed class UrlHandler(
    IUrlRepository urlRepository,
    IUrlService urlService,
    ICurrentUserService currentUserService,
    IUserRepository userRepository) :
    IRequestHandler<CreateUrlCommand, IResult>,
    IRequestHandler<DeleteUrlCommand, IResult>
{
    public async ValueTask<IResult> Handle(CreateUrlCommand request, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsUrl(request.LongUrl, "LongUrl", "Formato de url inválido.")
            .IsNotNullOrEmpty(request.LongUrl, "LongUrl", "A Url a ser encurtada é obrigatória.");

        if (!contract.IsValid) return new Result(ResultStatus.ValidationError, false, "Dados inválidos", contract);

        var userId = currentUserService.GetUserId();

        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);

        if (user == null)
            return new Result(ResultStatus.Unauthorized, false, "Usuário não consta no banco de dados.");

        // Se o usuário especificou uma URL customizada, verifica se já existe
        if (!string.IsNullOrEmpty(request.WantedShortUrl))
        {
            var exists = await urlRepository.GetUrlByShortUrlAsync(request.WantedShortUrl, cancellationToken);
            if (exists != null)
                return new Result(ResultStatus.Conflict, false, "A Url Já está em uso.");

            var customUrl = new Url(user, request.LongUrl, request.WantedShortUrl);
            await urlRepository.SaveAsync(customUrl, cancellationToken);

            var customUrlDto = new UrlDTO
            {
                Id = customUrl.Id,
                LongUrl = customUrl.LongUrl,
                ShortUrl = customUrl.ShortUrl
            };
            return new Result(ResultStatus.Success, true, "Url cadastrado com sucesso", customUrlDto);
        }

        // Tenta gerar URL aleatória com retry em caso de colisão previsível.
        const int maxRetries = 5;
        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            var shortUrl = urlService.GenerateShortUrl();

            var alreadyUsed = await urlRepository.GetUrlByShortUrlAsync(shortUrl, cancellationToken);
            if (alreadyUsed is not null)
                continue;

            var url = new Url(user, request.LongUrl, shortUrl);
            await urlRepository.SaveAsync(url, cancellationToken);

            var urlDto = new UrlDTO
            {
                Id = url.Id,
                LongUrl = url.LongUrl,
                ShortUrl = url.ShortUrl
            };
            return new Result(ResultStatus.Success, true, "Url cadastrado com sucesso", urlDto);
        }

        return new Result(ResultStatus.Conflict, false,
            "Não foi possível gerar uma URL única após múltiplas tentativas. Tente novamente.");
    }

    public async ValueTask<IResult> Handle(DeleteUrlCommand request, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotNullOrEmpty(request.Id.ToString(), "Id", "O Id da Url a ser deletada é obrigatório.");

        if (!contract.IsValid) return new Result(ResultStatus.ValidationError, false, "Dados inválidos", contract);

        var userId = currentUserService.GetUserId();

        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);

        if (user == null)
            return new Result(ResultStatus.Unauthorized, false, "Usuário não consta no banco de dados.");

        var url = await urlRepository.GetUrlByIdAsync(request.Id, cancellationToken);

        if (url is null)
            return new Result(ResultStatus.NotFound, false, "Url não encontrada.");

        if (url.Creator.Id != user.Id) return new Result(ResultStatus.Unauthorized, false, "Não autorizado.");

        await urlRepository.DeleteUrlAsync(url, cancellationToken);
        return new Result(ResultStatus.Success, true, "Url deletado com sucesso.");
    }
}