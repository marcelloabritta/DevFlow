using System.Security.Claims;

using DevFlow.API.Extensions;
using DevFlow.API.Requests;
using DevFlow.API.Responses;
using DevFlow.Application.Common;
using DevFlow.Application.Exceptions;
using DevFlow.Application.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _service;

    public ProjectsController(ProjectService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Leader,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var id = await _service.CreateAsync(request.Name, request.Description, userId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var project = await _service.GetByIdAsync(id, cancellationToken);
        var response = ProjectResponse.FromEntity(project);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(page, pageSize, cancellationToken);

        var response = new PagedResult<ProjectWithOwnerResponse>(
                    result.Items.Select(ProjectWithOwnerResponse.FromModel),
                    result.Page,
                    result.PageSize,
                    result.TotalItems);

        return Ok(response);
    }

    [Authorize(Roles = "Leader,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request.Name, request.Description, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
