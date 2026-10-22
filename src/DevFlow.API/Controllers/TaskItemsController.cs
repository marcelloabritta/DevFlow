using DevFlow.API.Requests;
using DevFlow.API.Responses;
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
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var taskItem = await _service.GetByIdAsync(id, cancellationToken);
        var response = TaskItemResponse.FromEntity(taskItem);
        return Ok(response);
    }
}
