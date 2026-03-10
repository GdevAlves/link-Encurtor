namespace URLapi.Application.DTOs.UserDTO;

public class UserAuthorizedDTO
{
    public required string Email { get; set; }
    public Guid Id { get; set; }
    public string? Name { get; set; }
}