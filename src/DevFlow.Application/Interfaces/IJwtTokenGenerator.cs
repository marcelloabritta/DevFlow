using DevFlow.Domain.Entities;

namespace DevFlow.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(User user);
}
