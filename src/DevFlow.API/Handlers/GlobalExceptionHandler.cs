using DevFlow.Application.Exceptions;
using DevFlow.Domain.Exceptions;

using Microsoft.AspNetCore.Diagnostics;

namespace DevFlow.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int? statusCode = exception switch
        {
            DomainException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            OperationCanceledException => StatusCodes.Status499ClientClosedRequest,
            ConflictException => StatusCodes.Status409Conflict,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            _ => null
        };

        if (statusCode is null)
        {
            return false;
        }

        httpContext.Response.StatusCode = statusCode.Value;

        if (statusCode == StatusCodes.Status499ClientClosedRequest)
        {
            return true;
        }

        await httpContext.Response.WriteAsJsonAsync(new { message = exception.Message }, cancellationToken);

        return true;
    }
}
