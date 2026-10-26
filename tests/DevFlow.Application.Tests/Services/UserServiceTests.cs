using DevFlow.Application.Exceptions;
using DevFlow.Application.Interfaces;
using DevFlow.Application.Services;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;
using DevFlow.Domain.Exceptions;

using Moq;

namespace DevFlow.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _service = new UserService(_userRepositoryMock.Object, _passwordHasherMock.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateAsync_WithEmptyPassword_ThrowsDomainException(string? password)
    {
        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(
            name: "Valid name",
            email: "user@email.com",
            password: password!,
            cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WithShortPassword_ThrowsDomainException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(
            name: "Valid name",
            email: "user@email.com",
            password: new string('a', 7),
            cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WithExistingEmail_ThrowsConflictException()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(
            name: "Valid name",
            email: "user@email.com",
            password: new string('a', 8),
            cancellationToken: CancellationToken.None));

        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesUser()
    {
        // Arrange
        var password = new string('a', 8);
        var passwordHash = "hashed-password";
        _userRepositoryMock.Setup(repo => repo.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);
        _passwordHasherMock.Setup(hasher => hasher.Hash(password))
                           .Returns(passwordHash);

        // Act
        var result = await _service.CreateAsync(name: "User Name", email: "user@email.com", password: password, cancellationToken: CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _userRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<User>(u => u.PasswordHash == passwordHash && u.Role == UserRole.Member),
            It.IsAny<CancellationToken>()
            ), Times.Once);
    }
}
