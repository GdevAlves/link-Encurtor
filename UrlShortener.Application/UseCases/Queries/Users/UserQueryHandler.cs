using System.Net;
using Flunt.Notifications;
using Flunt.Validations;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.UserDTO;
using UrlShortener.Application.UseCases.Commands;
using UrlShortener.Domain.IRepositories;

namespace UrlShortener.Application.UseCases.Queries.Users;

public sealed class UserQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserByIdQuery, IResult>
{
    public async ValueTask<IResult> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotEmpty(query.UserId, "UserId", "O ID do usuário é obrigatório");

        if (!contract.IsValid)
            return new Result(HttpStatusCode.BadRequest, false, "Validação falhou", contract.Notifications);

        try
        {
            var user = await userRepository.GetUserByIdAsync(query.UserId, cancellationToken);
            if (user == null) return new Result(HttpStatusCode.NotFound, false, "Usuário não encontrado.");

            return new Result(HttpStatusCode.OK, true, "Usuário encontrado.", new UserAuthorizedDTO
            {
                Id = user.Id,
                Email = user.Email.Address,
                Name = user.Name
            });
        }
        catch (Exception)
        {
            return new Result(HttpStatusCode.InternalServerError, false, "Erro ao buscar usuário.");
        }
    }
}