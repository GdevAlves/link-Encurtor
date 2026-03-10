using System.ComponentModel.DataAnnotations;

namespace URLapi.Application.DTOs.UserDTO;

public class UserToAuthDTO
{
    [EmailAddress] public required string Email { get; set; }

    public required string Password { get; set; }
}