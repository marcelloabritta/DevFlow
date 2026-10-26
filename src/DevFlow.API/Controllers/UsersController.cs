using DevFlow.API.Requests;
using DevFlow.Application.Services;

using Microsoft.AspNetCore.Mvc;

namespace DevFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request.Name, request.Email, request.Password, cancellationToken);
        return Created($"/api/users/{id}", id);
    }
}
