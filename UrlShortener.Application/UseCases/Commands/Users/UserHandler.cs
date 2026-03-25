using Flunt.Notifications;
using Flunt.Validations;
using Mediator;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.UserDTO;
using UrlShortener.Application.Enums;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.IRepositories;
using UrlShortener.Domain.IServices;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Application.UseCases.Commands.Users;

public sealed class UserCommandHandler(
    IUserRepository userRepository,
    IVerifyUserService verifyUserService,
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
        var normalizedEmail = command.Email.Trim();

        // Validate command
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotNullOrEmpty(command.Name, "Name", "O nome é obrigatório")
            .IsGreaterThan(command.Name, 3, "Name", "O nome deve conter pelo menos 3 caracteres")
            .IsNotNullOrEmpty(normalizedEmail, "Email", "O email é obrigatório")
            .IsEmail(normalizedEmail, "Email", "O email é inválido")
            .IsGreaterOrEqualsThan(command.Password, 6, "Password", "A senha deve conter pelo menos 6 caracteres.")
            .IsNotNullOrEmpty(command.Password, "Password", "A senha é obrigatória");

        if (!contract.IsValid)
            return new Result(ResultStatus.ValidationError, false, "Validação falhou", contract.Notifications);

        // Create value objects and entity
        var email = new Email(normalizedEmail!);
        var password = Password.FromPlainText(command.Password, passwordHasher);
        var user = new User(email, password, command.Name);

        // Check if email is already registered
        var existing = await userRepository.GetUserByEmailAsync(email.Address, cancellationToken);
        if (existing != null)
            return new Result(ResultStatus.Conflict, false, "Email já está em uso.");

        // Persist user
        await userRepository.SaveAsync(user, cancellationToken);

        // Send verification email
        await verifyUserService.SendVerificationAsync(user, cancellationToken);

        return new Result(ResultStatus.Success, true, "Usuário cadastrado com sucesso.", new UserAuthorizedDTO
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
            return new Result(ResultStatus.ValidationError, false, "Validação falhou", contract.Notifications);

        var user = await userRepository.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user == null) return new Result(ResultStatus.NotFound, false, "Usuário não encontrado.");

        // Deletar o usuário
        await userRepository.DeleteAsync(user, cancellationToken);

        return new Result(ResultStatus.Success, true, "Usuário deletado com sucesso.");
    }

    public async ValueTask<IResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        // Validate
        var contract = new Contract<Notification>()
            .Requires()
            .IsEmail(command.Email, "Email", "O email é inválido")
            .IsNotNullOrEmpty(command.Password, "Password", "A senha é obrigatória");

        if (!contract.IsValid)
            return new Result(ResultStatus.ValidationError, false, "Validação falhou", contract.Notifications);

        var user = await userRepository.GetUserByEmailAsync(command.Email, cancellationToken);
        if (user == null) return new Result(ResultStatus.Unauthorized, false, "Email ou senha incorretos.");

        if (!user.Email.Verification.Verified)
            return new Result(ResultStatus.Unauthorized, false,
                "Usuário não verificado verifique seu email e tente novamente.");

        // Verify password
        if (!user.PasswordHash.Verify(command.Password, passwordHasher))
            return new Result(ResultStatus.Unauthorized, false, "Email ou senha incorretos.");

        // Create user DTO and generate token
        var userDto = new UserAuthorizedDTO
        {
            Id = user.Id,
            Email = user.Email.Address,
            Name = user.Name
        };

        var token = authService.GenerateJwtToken(user);

        return new Result(ResultStatus.Success, true, "Login realizado com sucesso.", new
        {
            Token = token,
            User = userDto
        });
    }

    public async ValueTask<IResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        // Validate
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotEmpty(command.UserId, "UserId", "O ID do usuário é obrigatório");

        if (!string.IsNullOrEmpty(command.Email)) contract.IsEmail(command.Email, "Email", "O email é inválido");

        if (!contract.IsValid)
            return new Result(ResultStatus.ValidationError, false, "Validação falhou", contract.Notifications);

        // Find user
        var user = await userRepository.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user == null) return new Result(ResultStatus.NotFound, false, "Usuário não encontrado.");

        // Update fields if provided
        var hasChanges = false;

        if (!string.IsNullOrEmpty(command.Name) && command.Name != user.Name)
        {
            user.Name = command.Name;
            hasChanges = true;
        }

        if (!string.IsNullOrEmpty(command.Email) && command.Email != user.Email.Address)
        {
            // Check if new email is already in use
            var existing = await userRepository.GetUserByEmailAsync(command.Email, cancellationToken);
            if (existing != null && existing.Id != command.UserId)
                return new Result(ResultStatus.Conflict, false, "Email já está em uso por outro usuário.");

            user.Email = new Email(command.Email);
            hasChanges = true;
        }

        if (!hasChanges)
            return new Result(ResultStatus.Success, true, "Nenhuma alteração detectada.", new UserAuthorizedDTO
            {
                Id = user.Id,
                Email = user.Email.Address,
                Name = user.Name
            });

        await userRepository.UpdateAsync(user, cancellationToken);

        return new Result(ResultStatus.Success, true, "Usuário atualizado com sucesso.", new UserAuthorizedDTO
        {
            Id = user.Id,
            Email = user.Email.Address,
            Name = user.Name
        });
    }

    public async ValueTask<IResult> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByVerificationCodeAsync(request.Token, cancellationToken);

        if (user == null)
            return new Result(ResultStatus.NotFound, false, "Requisição inválida.");

        if (user.Email.Verification.VerifyHashCodeExpiration < DateTime.UtcNow)
            return new Result(ResultStatus.Unauthorized, false, "Código já expirado.");

        if (!user.Email.Verification.Verified)
            user.Email.Verification.Verified = true;

        await userRepository.UpdateAsync(user, cancellationToken);

        return new Result(ResultStatus.Success, true, "Usuário verificado com sucesso.",
            new string("Usuário verificado com sucesso."));
    }
}