using URLapi.Domain.ValueObjects;

namespace URLapi.Domain.Entities;

public class User : Entity
{
    private User()
    {
    }

    public User(Email emailAdress, Password password, string? name)
    {
        Email = emailAdress;
        PasswordHash = password;
        Name = name;
    }

    public string? Name { get; set; }
    public Password PasswordHash { get; set; }
    public Email Email { get; set; }
}