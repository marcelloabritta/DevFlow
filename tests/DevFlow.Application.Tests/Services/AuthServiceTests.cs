using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Services;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;

using Moq;

namespace DevFlow.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _tokenGeneratorMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _service = new AuthService(_userRepositoryMock.Object, _passwordHasherMock.Object, _tokenGeneratorMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithNonexistentEmail_ThrowsUnauthorizedException()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginAsync(
            email: "user@email.com",
            password: "password123",
            cancellationToken: CancellationToken.None));

        _tokenGeneratorMock.Verify(generator => generator.Generate(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = new User("user name", "user@email.com", "passwordHash", UserRole.Member);
        var password = "password123";

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
        _passwordHasherMock.Setup(hasher => hasher.VerifyPassword(password, user.PasswordHash))
                           .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginAsync(
            email: user.Email,
            password: password,
            cancellationToken: CancellationToken.None));

        _passwordHasherMock.Verify(hasher => hasher.VerifyPassword(password, user.PasswordHash), Times.Once);
        _tokenGeneratorMock.Verify(generator => generator.Generate(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var user = new User("user name", "user@email.com", "passwordHash", UserRole.Member);
        var password = "password123";
        var token = "jwt-token";

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
        _passwordHasherMock.Setup(hasher => hasher.VerifyPassword(password, user.PasswordHash))
                           .Returns(true);
        _tokenGeneratorMock.Setup(generator => generator.Generate(user))
                           .Returns(token);

        // Act
        var result = await _service.LoginAsync(
            email: user.Email,
            password: password,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(token, result);

        _passwordHasherMock.Verify(hasher => hasher.VerifyPassword(password, user.PasswordHash), Times.Once);
        _tokenGeneratorMock.Verify(generator => generator.Generate(user), Times.Once);
    }
}
