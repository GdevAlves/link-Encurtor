using System.Net;
using Flunt.Notifications;
using Flunt.Validations;
using Mediator;
using URLapi.Application.Abstractions;
using URLapi.Application.DTOs.UserDTO;
using URLapi.Domain.Entities;
using URLapi.Domain.IRepositories;
using URLapi.Domain.IServices;
using URLapi.Domain.ValueObjects;

namespace URLapi.Application.UseCases.Commands.Users;

public sealed class UserCommandHandler(
    IUserRepository userRepository,
    IVerificateUserService verificateUserService,
    IPasswordHasher passwordHasher,
    IAuthService authService)
    : IRequestHandler<CreateUserCommand, IResult>,
        IRequestHandler<UpdateUserCommand, IResult>,
        IRequestHandler<DeleteUserCommand, IResult>,
        IRequestHandler<LoginUserCommand, IResult>,
        IRequestHandler<VerifyUserCommand, IResult>
{
    public async ValueTask<IResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Validate command
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotNullOrEmpty(command.Name, "Name", "O nome é obrigatório")
            .IsGreaterThan(command.Name, 3, "Name", "O nome deve conter pelo menos 3 caracteres")
            .IsEmail(command.Email.Trim(), "Email", "O email é inválido")
            .IsGreaterOrEqualsThan(command.Password, 6, "Password", "A senha deve conter pelo menos 6 caracteres.")
            .IsNotNullOrEmpty(command.Password, "Password", "A senha é obrigatória");

        if (!contract.IsValid)
            return new Result(HttpStatusCode.BadRequest, false, "Validação falhou", contract.Notifications);

        // Create value objects and entity
        Email email;
        Password password;
        User user;

        try
        {
            email = new Email(command.Email);
            password = Password.FromPlainText(command.Password, passwordHasher);
            user = new User(email, password, command.Name);
        }
        catch (Exception)
        {
            return new Result(HttpStatusCode.BadRequest, false, "Dados do usuário inválidos.");
        }

        // Check if email is already registered
        try
        {
            var existing = await userRepository.GetUserByEmailAsync(command.Email, cancellationToken);
            if (existing != null)
                return new Result(HttpStatusCode.Conflict, false, "Email já está em uso.");
        }
        catch (Exception e)
        {
            return new Result(HttpStatusCode.InternalServerError, false, "Erro interno do servidor");
        }

        // Persist user
        try
        {
            await userRepository.SaveAsync(user, cancellationToken);
        }
        catch (Exception)
        {
            return new Result(HttpStatusCode.InternalServerError, false, "Não foi possível persistir os dados.");
        }

        // Send verification email
        try
        {
            await verificateUserService.SendVerificationAsync(user, cancellationToken);
        }
        catch
        {
            return new Result(HttpStatusCode.Created, true,
                "Usuário cadastrado, porém falha ao enviar email de verificação.", new UserAuthorizedDTO
                {
                    Id = user.Id,
                    Email = email.Address,
                    Name = user.Name
                });
        }

        return new Result(HttpStatusCode.Created, true, "Usuário cadastrado com sucesso.", new UserAuthorizedDTO
        {
            Id = user.Id,
            Email = email.Address,
            Name = user.Name
        });
    }

    public async ValueTask<IResult> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        // Validate
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotEmpty(command.UserId, "UserId", "O ID do usuário é obrigatório");

        if (!contract.IsValid)
            return new Result(HttpStatusCode.BadRequest, false, "Validação falhou", contract.Notifications);

        try
        {
            var user = await userRepository.GetUserByIdAsync(command.UserId, cancellationToken);
            if (user == null) return new Result(HttpStatusCode.NotFound, false, "Usuário não encontrado.");

            // Deletar o usuário
            await userRepository.DeleteAsync(user, cancellationToken);

            return new Result(HttpStatusCode.OK, true, "Usuário deletado com sucesso.");
        }
        catch (Exception ex)
        {
            return new Result(HttpStatusCode.InternalServerError, false, $"Erro ao deletar usuário: {ex.Message}");
        }
    }

    public async ValueTask<IResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        // Validate
        var contract = new Contract<Notification>()
            .Requires()
            .IsEmail(command.Email, "Email", "O email é inválido")
            .IsNotNullOrEmpty(command.Password, "Password", "A senha é obrigatória");

        if (!contract.IsValid)
            return new Result(HttpStatusCode.BadRequest, false, "Validação falhou", contract.Notifications);

        try
        {
            var user = await userRepository.GetUserByEmailAsync(command.Email, cancellationToken);
            if (user == null) return new Result(HttpStatusCode.Unauthorized, false, "Email ou senha incorretos.");

            if (!user.Email.Verification.Verified)
                return new Result(HttpStatusCode.Unauthorized, false,
                    "Usuário não verificado verifique seu email e tente novamente.");

            // Verify password
            if (!user.PasswordHash.Verify(command.Password, passwordHasher))
                return new Result(HttpStatusCode.Unauthorized, false, "Email ou senha incorretos.");

            // Create user DTO and generate token
            var userDto = new UserAuthorizedDTO
            {
                Id = user.Id,
                Email = user.Email.Address,
                Name = user.Name
            };

            var token = authService.GenerateJwtToken(user);

            return new Result(HttpStatusCode.OK, true, "Login realizado com sucesso.", new
            {
                Token = token,
                User = userDto
            });
        }
        catch (Exception ex)
        {
            return new Result(HttpStatusCode.InternalServerError, false, $"Erro ao realizar login: {ex.Message}");
        }
    }

    public async ValueTask<IResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        // Validate
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotEmpty(command.UserId, "UserId", "O ID do usuário é obrigatório");

        if (!string.IsNullOrEmpty(command.Email)) contract.IsEmail(command.Email, "Email", "O email é inválido");

        if (!contract.IsValid)
            return new Result(HttpStatusCode.BadRequest, false, "Validação falhou", contract.Notifications);

        // Find user
        User? user;
        try
        {
            user = await userRepository.GetUserByIdAsync(command.UserId, cancellationToken);
            if (user == null) return new Result(HttpStatusCode.NotFound, false, "Usuário não encontrado.");
        }
        catch (Exception)
        {
            return new Result(HttpStatusCode.InternalServerError, false, "Erro ao buscar usuário.");
        }

        // Update fields if provided
        try
        {
            var hasChanges = false;

            if (!string.IsNullOrEmpty(command.Name) && command.Name != user.Name)
            {
                var nameProperty = user.GetType().GetProperty("Name");
                if (nameProperty != null && nameProperty.CanWrite)
                {
                    nameProperty.SetValue(user, command.Name);
                    hasChanges = true;
                }
            }

            if (!string.IsNullOrEmpty(command.Email) && command.Email != user.Email.Address)
            {
                // Check if new email is already in use
                var existing = await userRepository.GetUserByEmailAsync(command.Email, cancellationToken);
                if (existing != null && existing.Id != command.UserId)
                    return new Result(HttpStatusCode.Conflict, false, "Email já está em uso por outro usuário.");

                var newEmail = new Email(command.Email);
                var emailProperty = user.GetType().GetProperty("Email");
                if (emailProperty != null && emailProperty.CanWrite)
                {
                    emailProperty.SetValue(user, newEmail);
                    hasChanges = true;
                }
            }

            if (!hasChanges)
                return new Result(HttpStatusCode.OK, true, "Nenhuma alteração detectada.", new UserAuthorizedDTO
                {
                    Id = user.Id,
                    Email = user.Email.Address,
                    Name = user.Name
                });

            await userRepository.UpdateAsync(user, cancellationToken);

            return new Result(HttpStatusCode.OK, true, "Usuário atualizado com sucesso.", new UserAuthorizedDTO
            {
                Id = user.Id,
                Email = user.Email.Address,
                Name = user.Name
            });
        }
        catch (Exception ex)
        {
            return new Result(HttpStatusCode.InternalServerError, false, $"Erro ao atualizar usuário: {ex.Message}");
        }
    }

    public async ValueTask<IResult> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByVerificationCodeAsync(request.Token, cancellationToken);

        if (user == null)
            return new Result(HttpStatusCode.NotFound, false, "Requisição inválida.");

        if (user.Email.Verification.VerifyHashCodeExpiration < DateTime.UtcNow)
            return new Result(HttpStatusCode.Unauthorized, false, "Código já expirado.");

        if (!user.Email.Verification.Verified)
            user.Email.Verification.Verified = true;

        await userRepository.UpdateAsync(user, cancellationToken);

        return new Result(HttpStatusCode.OK, true, "Usuário verificado com sucesso.",
            new string("Usuário verificado com sucesso."));
    }
}