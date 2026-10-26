using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using DevFlow.Application.Interfaces;
using DevFlow.Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DevFlow.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private const int TokenExpirationHours = 1;
    private readonly string _key;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not found.");
    }

    public string Generate(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var keyBytes = Encoding.UTF8.GetBytes(_key);
        var key = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(TokenExpirationHours),
                    signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
