using Microsoft.AspNetCore.Mvc;
using TaskManager.Application;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] AuthDTO dto)
    {
        try
        {
            var result = await _authService.RegisterUser(dto.Email, dto.Password);
            return StatusCode(201, result);
        }
        catch(Exception ex)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Title = "User could not be registered."
            };
        
            return BadRequest(problemDetails);
        }
    } 

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] AuthDTO dto)
    {
        try
        {
            var result = await _authService.LoginUser(dto.Email, dto.Password);
            return Ok(result);
        }
        catch(Exception ex)
        {
            var problemDetails = new ProblemDetails()
            {
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized,
                Title = "User is not authorized to login."
            };
        
            return Unauthorized(problemDetails);
        }
    }

}