using URLapi.Domain.Entities;

namespace URLapi.Domain.IServices;

public interface IVerifyUserService
{
    Task SendVerificationAsync(User user, CancellationToken cancellationToken);
}