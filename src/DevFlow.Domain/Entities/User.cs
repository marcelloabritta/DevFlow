using DevFlow.Domain.Common;
using DevFlow.Domain.Enums;
using DevFlow.Domain.Exceptions;

namespace DevFlow.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }

    public User(string name, string email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("The user name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("The email is required.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("The password is required.");
        }

        if (!email.Contains('@'))
        {
            throw new DomainException("Email must contain '@'.");
        }

        if (!Enum.IsDefined(role))
        {
            throw new DomainException("Invalid user role.");
        }

        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }
}
