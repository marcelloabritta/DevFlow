using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;

namespace DevFlow.Application.Services;

public class AuthService
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        return _tokenGenerator.Generate(user);
    }
}
