namespace DevFlow.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool VerifyPassword(string password, string passwordHash);
}
