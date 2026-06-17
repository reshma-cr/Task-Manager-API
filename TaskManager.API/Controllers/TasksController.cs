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
        var tasks = await _taskService.GetAllTasks();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTask(Guid id)
    {
        var task = await _taskService.GetTask(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask([FromBody] TaskItem task)
    {
        if (task == null || string.IsNullOrWhiteSpace(task.Title) || string.IsNullOrWhiteSpace(task.Description))
        {
            return BadRequest("Task title and description cannot be empty.");
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
            return NotFound();
        }
        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult> ToggleTask(Guid id)
    {
        var toggledTask = await _taskService.ToggleTask(id);
        if (!toggledTask)
        {
            return NotFound();
        }
        return NoContent();
    }

}
