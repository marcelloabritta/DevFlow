using DevFlow.API.Requests;
using DevFlow.API.Responses;
using DevFlow.Application.Common;
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

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid projectId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(projectId, page, pageSize, cancellationToken);

        var response = new PagedResult<TaskItemResponse>(
                    result.Items.Select(TaskItemResponse.FromEntity),
                    result.Page,
                    result.PageSize,
                    result.TotalItems);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskItemRequest request, CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(
            id,
            request.ToCommand(),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
