using DevFlow.API.Requests;
using DevFlow.API.Responses;
using DevFlow.Application.Common;
using DevFlow.Application.Services;

using Microsoft.AspNetCore.Mvc;

namespace DevFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _service;

    public ProjectsController(ProjectService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectRequest request)
    {
        var id = await _service.CreateAsync(request.Name, request.Description, request.OwnerId);
        return Created($"/api/projects/{id}", id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await _service.GetByIdAsync(id);
        var response = ProjectResponse.FromEntity(project);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var result = await _service.GetAllAsync(page, pageSize);

        var response = new PagedResult<ProjectResponse>(
                    result.Items.Select(ProjectResponse.FromEntity),
                    result.Page,
                    result.PageSize,
                    result.TotalItems);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectRequest request)
    {
        await _service.UpdateAsync(id, request.Name, request.Description);
        return NoContent();
    }
}
