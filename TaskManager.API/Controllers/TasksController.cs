using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain;
using TaskManager.Application;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllTasks(bool? status=null, string? search = null){
        var tasks = await _taskService.GetAllTasks(status, search);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(Guid id)
    {
        var task = await _taskService.GetTask(id);        
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
    public async Task<ActionResult<TaskItem>> CreateTask([FromBody] TaskItem task)
    {
        if (task == null || string.IsNullOrWhiteSpace(task.Title) || string.IsNullOrWhiteSpace(task.Description))
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = $"title and description cannot be empty",
                Status = StatusCodes.Status400BadRequest,
                Title = "Resource cannot be empty."
            };
        
            return BadRequest(problemDetails);
        }
        var createdTask = await _taskService.CreateTask(task.Title, task.Description);
        return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(Guid id)
    {
        var deletedTask = await _taskService.DeleteTask(id);
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
        var toggledTask = await _taskService.ToggleTask(id);
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
        var task = await _taskService.GetTask(id);
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
        await _taskService.UpdateTask(id, dto.Title, dto.Description, dto.IsCompleted);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchTask([FromBody] PatchTaskDTO dto, Guid id)
    {
        var task = await _taskService.GetTask(id);
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
        task = await _taskService.PatchTask(id, dto.Title, dto.Description, dto.IsCompleted);
        return Ok(task);
    }
}
