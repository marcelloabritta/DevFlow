using DevFlow.Domain.Common;
using DevFlow.Domain.Enums;

namespace DevFlow.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }

    public User(string name, string email, string passwordHash, UserRole role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        if (!email.Contains('@'))
        {
            throw new ArgumentException("Email must contain '@'.");
        }

        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }
}
