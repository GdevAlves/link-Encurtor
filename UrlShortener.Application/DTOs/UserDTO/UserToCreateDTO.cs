using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Application.DTOs.UserDTO;

public class UserToCreateDTO
{
    public string? Name { get; set; }

    [EmailAddress] public required string Email { get; set; }

    public required string Password { get; set; }
}