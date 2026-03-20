using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.IServices;

public interface IVerifyUserService
{
    Task SendVerificationAsync(User user, CancellationToken cancellationToken);
}