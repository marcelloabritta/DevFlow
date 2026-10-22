using DevFlow.API.Requests;
using DevFlow.Application.Services;

using Microsoft.AspNetCore.Mvc;

namespace DevFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskItemsController : ControllerBase
{
    private readonly TaskItemService _service;

    public TaskItemsController(TaskItemService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskItemRequest request, CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request.ToCommand(), cancellationToken);
        return Created($"/api/taskitems/{id}", id);
    }
}
