using URLapi.Domain.Entities;

namespace URLapi.Domain.IServices;

public interface IAuthService
{
    string GenerateJwtToken(User user);
}