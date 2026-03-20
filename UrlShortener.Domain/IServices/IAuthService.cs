using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.IServices;

public interface IAuthService
{
    string GenerateJwtToken(User user);
}