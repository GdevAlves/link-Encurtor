using URLapi.Domain.DTOs.UserDTO;

namespace URLapi.Domain.IServices;

public interface IAuthService
{
    string GenerateJwtToken(UserAuthorizedDTO user);
}