using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain;
using TaskManager.Application;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllTasks(bool? status=null, string? search = null){
        Guid userId = GetCurrentUserId();
        var tasks = await _taskService.GetAllTasks(userId, status, search);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(Guid id)
    {
        var userid = GetCurrentUserId();
        var task = await _taskService.GetTask(userid, id);        
        if (task == null)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"task with ID: {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found."
            };
        
            return NotFound(problemDetails);
        }
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskDTO dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Title))
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"title and description cannot be empty",
                Status = StatusCodes.Status400BadRequest,
                Title = "Resource cannot be empty."
            };
        
            return BadRequest(problemDetails);
        }
        var userId = GetCurrentUserId();
        var createdTask = await _taskService.CreateTask(dto.Title, dto.Notes, userId, dto.DueDate, dto.DueTime, dto.ReminderAt, dto.ProjectId);
        return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(Guid id)
    {
        var userId = GetCurrentUserId();
        var deletedTask = await _taskService.DeleteTask(id, userId);
        if (!deletedTask)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"task with ID: {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found."
            };
        
            return NotFound(problemDetails);
        }
        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult> ToggleTask(Guid id)
    {
        var userId = GetCurrentUserId();
        var toggledTask = await _taskService.ToggleTask(id, userId);
        if (!toggledTask)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"task with ID: {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found."
            };
        
            return NotFound(problemDetails);
        }
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTask([FromBody] UpdateTaskDTO dto, Guid id)
    {
        var userId = GetCurrentUserId();
        var task = await _taskService.GetTask(userId, id);
        if(task == null)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"task with ID: {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found."
            };
        
            return NotFound(problemDetails);
        }
        await _taskService.UpdateTask(id, userId, dto.Title, dto.Notes, dto.IsCompleted, dto.DueDate, dto.DueTime, dto.ReminderAt, dto.ProjectId);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchTask([FromBody] PatchTaskDTO dto, Guid id)
    {
        var userId = GetCurrentUserId();
        var task = await _taskService.GetTask(userId, id);
        if(task == null)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"task with ID: {id} was not found",
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found."
            };
        
            return NotFound(problemDetails);
        }
        task = await _taskService.PatchTask(id, userId, dto.Title, dto.Notes, dto.IsCompleted, dto.DueDate, dto.DueTime, dto.ReminderAt, dto.ProjectId);
        return Ok(task);
    }

    private Guid GetCurrentUserId()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return userId;
    }
}
