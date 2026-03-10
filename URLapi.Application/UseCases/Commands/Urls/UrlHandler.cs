using System.Net;
using Flunt.Notifications;
using Flunt.Validations;
using Mediator;
using URLapi.Application.Abstractions;
using URLapi.Application.DTOs.UrlDTO;
using URLapi.Domain.Entities;
using URLapi.Domain.IRepositories;
using URLapi.Domain.IServices;

namespace URLapi.Application.UseCases.Commands.Urls;

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
            .IsUrl(request.BigUrl, "BigUrl", "Formato de url inválido.")
            .IsNotNullOrEmpty(request.BigUrl, "BigUrl", "A Url a ser encurtada é obrigatória.");

        if (!contract.IsValid) return new Result(HttpStatusCode.BadRequest, false, "Dados inválidos", contract);

        var userId = currentUserService.GetUserId();

        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);

        if (user == null)
            return new Result(HttpStatusCode.Unauthorized, false, "Usuário não consta no banco de dados.");

        var shortUrl = !string.IsNullOrEmpty(request.WantedShortUrl)
            ? request.WantedShortUrl
            : urlService.GenerateShortUrl();

        var exists = await urlRepository.GetUrlByShortUrlAsync(shortUrl, cancellationToken);

        if (!string.IsNullOrEmpty(request.WantedShortUrl) && exists != null)
            return new Result(HttpStatusCode.Conflict, false, "A Url Já está em uso.");


        while (exists is not null)
        {
            shortUrl = urlService.GenerateShortUrl();
            exists = await urlRepository.GetUrlByShortUrlAsync(shortUrl, cancellationToken);
        }

        var url = new Url(user, request.BigUrl, shortUrl);

        await urlRepository.SaveAsync(url, cancellationToken);

        var urlDto = new UrlDTO
        {
            Id = url.Id,
            LongUrl = url.LongUrl,
            ShortUrl = url.ShortUrl
        };
        return new Result(HttpStatusCode.OK, true, "Url cadastrado com sucesso", urlDto);
    }

    public async ValueTask<IResult> Handle(DeleteUrlCommand request, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotNullOrEmpty(request.Id.ToString(), "Id", "O Id da Url a ser deletada é obrigatório.");

        if (!contract.IsValid) return new Result(HttpStatusCode.BadRequest, false, "Dados inválidos", contract);

        var userId = currentUserService.GetUserId();

        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);

        if (user == null)
            return new Result(HttpStatusCode.Unauthorized, false, "Usuário não consta no banco de dados.");

        var url = await urlRepository.GetUrlByIdAsync(request.Id, cancellationToken);

        if (url is null)
            return new Result(HttpStatusCode.NotFound, false, "Url não encontrada.");

        if (url.Creator.Id != user.Id) return new Result(HttpStatusCode.Unauthorized, false, "Não autorizado.");

        await urlRepository.DeleteUrlAsync(url, cancellationToken);
        return new Result(HttpStatusCode.OK, true, "Url deletado com sucesso.");
    }
}