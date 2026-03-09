using URLapi.Domain.Entities;

namespace URLapi.Domain.IServices;

public interface IVerificateUserService
{
    Task SendVerificationAsync(User user, CancellationToken cancellationToken);
}