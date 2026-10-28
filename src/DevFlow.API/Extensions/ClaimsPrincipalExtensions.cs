using System.Security.Claims;

using DevFlow.Application.Exceptions;

namespace DevFlow.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            throw new UnauthorizedException("Invalid token.");
        }
        return userId;
    }
}
