using URLapi.Domain.Entities;

namespace URLapi.Domain.IRepositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task SaveAsync(User user, CancellationToken cancellationToken);

    Task UpdateAsync(User user, CancellationToken cancellationToken);

    Task DeleteAsync(User user, CancellationToken cancellationToken);

    Task<User?> GetUserByVerificationCodeAsync(Guid verificationCode, CancellationToken cancellationToken);
}