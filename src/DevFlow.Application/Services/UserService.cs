using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;
using DevFlow.Domain.Exceptions;

namespace DevFlow.Application.Services;

public class UserService
{
    private const int MinPasswordLength = 8;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> CreateAsync(string name, string email, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException("The password is required.");
        }

        if (password.Length < MinPasswordLength)
        {
            throw new DomainException($"The password must have at least {MinPasswordLength} characters.");
        }

        var emailExists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        if (emailExists)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var passwordHash = _passwordHasher.Hash(password);
        var user = new User(name, email, passwordHash, UserRole.Member);
        await _userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}
