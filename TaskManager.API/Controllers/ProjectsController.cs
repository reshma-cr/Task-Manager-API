using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAllProjects()
    {
        var userId = GetCurrentUserId();
        var projects = await _projectService.GetAllProjects(userId);
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetProject(Guid id)
    {
        var userId = GetCurrentUserId();
        var project = await _projectService.GetProject(id, userId);
        if (project == null)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"project with ID: {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found."
            };
        
            return NotFound(problemDetails);
        }
        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult> CreateProject([FromBody] ProjectDTO dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"name cannot be empty",
                Status = StatusCodes.Status400BadRequest,
                Title = "Resource cannot be empty."
            };

            return BadRequest(problemDetails);
        }
        var userId = GetCurrentUserId();
        var createdProject = await _projectService.CreateProject(dto.Name, userId);
        return CreatedAtAction(nameof(GetProject), new {id = createdProject.Id}, createdProject);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var project = await _projectService.DeleteProject(id, userId);
            if (!project)
            {
                var problemDetails = new ProblemDetails()
                {
                    Detail = $"project with ID: {id} was not found",
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource not found."
                };
        
                return NotFound(problemDetails);
            }
            return NoContent();
        }
        catch(InvalidOperationException ex)
        {
            var problemDetails = new ProblemDetails()
                {
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Operation"
                };
            return BadRequest(problemDetails);
        }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchProject([FromBody] ProjectDTO dto, Guid id)
    {
        var userId = GetCurrentUserId();
        var project = await _projectService.GetProject(id, userId);
        if(project == null)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"project with ID: {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found."
            };
        
            return NotFound(problemDetails);
        }
        project = await _projectService.PatchProject(id, dto.Name, userId);
        return Ok(project);
    }

    private Guid GetCurrentUserId()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return userId;
    }
}