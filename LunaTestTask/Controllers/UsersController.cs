using LunaTestTask.Models;
using LunaTestTask.Models.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LunaTestTask.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly UserContext _context;

    public UsersController(UserContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(UserModel userRequest)
    {
        if (await _context.Users.AnyAsync(i => i.Username == userRequest.Username))
        {
            return Conflict("Username is already used!");
        }

        if (await _context.Users.AnyAsync(i => i.EMail == userRequest.EMail))
        {
            return Conflict("Email is already used!");
        }

        userRequest.CreatedAt = DateTime.UtcNow;
        userRequest.UpdatedAt = DateTime.UtcNow;

        _context.Users.Add(userRequest);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(UserModel userRequest)
    {
        var existingUser = await _context.Users.AnyAsync(i => i.Username == userRequest.Username || i.EMail == userRequest.EMail);
        if (!existingUser)
        {
            return NotFound("That username or email doesn't exist!");
        }

        return NoContent();
    }
}