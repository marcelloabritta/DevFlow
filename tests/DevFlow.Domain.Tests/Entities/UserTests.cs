using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DevFlow.Domain.Entities;
using DevFlow.Domain.Enums;
using DevFlow.Domain.Exceptions;

using Xunit;

namespace DevFlow.Domain.Tests.Entities;

public class UserTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ThrowsDomainException(string? name)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateUser(name: name!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidEmail_ThrowsDomainException(string? email)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateUser(email: email!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidPasswordHash_ThrowsDomainException(string? passwordHash)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateUser(passwordHash: passwordHash!));
    }

    [Fact]
    public void Constructor_WithEmailWithoutAtSign_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateUser(email: "useremail.com"));
    }

    [Theory]
    [InlineData((UserRole)0)]
    [InlineData((UserRole)(-1))]
    public void Constructor_WithInvalidRole_ThrowsDomainException(UserRole role)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => CreateUser(role: role));
    }

    [Fact]
    public void Constructor_WithValidData_CreatesUser()
    {
        // Act
        var user = CreateUser();

        // Assert
        Assert.Equal("Valid name", user.Name);
        Assert.Equal("user@email.com", user.Email);
        Assert.Equal("Password Hash", user.PasswordHash);
        Assert.Equal(UserRole.Member, user.Role);
    }

    private static User CreateUser(
        string name = "Valid name",
        string email = "user@email.com",
        string passwordHash = "Password Hash",
        UserRole role = UserRole.Member)
    {
        return new User(
            name: name,
            email: email,
            passwordHash: passwordHash,
            role: role
        );
    }
}
