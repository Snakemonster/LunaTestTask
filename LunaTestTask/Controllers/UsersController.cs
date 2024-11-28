using Microsoft.AspNetCore.Mvc;

namespace LunaTestTask.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser()
    {
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser()
    {
        return NoContent();
    }
}