using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NLTaskController : ControllerBase
{
    private readonly INLTaskService _nlTaskService;
    public NLTaskController(INLTaskService nLTaskService)
    {
        _nlTaskService = nLTaskService;
    }
    [HttpPost]
    public async Task<ActionResult> CreateNLTask([FromBody]NLTaskDTO dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _nlTaskService.NLToJson(dto.Input, userId);
            if(result == null)
            {
                var problemDetails = new ProblemDetails()
                {
                    Detail = "unable to create task at the moment",
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Unable to create task"
                };
                return BadRequest(problemDetails);
            }
            return Ok(result);
        }
        catch (System.Exception ex)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Title = "AI Service Error"
            };
            return BadRequest(problemDetails);
        }
    }

    private Guid GetCurrentUserId()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return userId;
    }
}