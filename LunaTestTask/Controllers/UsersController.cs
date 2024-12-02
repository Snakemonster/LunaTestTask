using System.Text.RegularExpressions;
using LunaTestTask.Models;
using LunaTestTask.Models.Contexts;
using LunaTestTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LunaTestTask.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly UserContext _context;
    private AuthService _authService;
    public const int MinPasswordLength = 8;

    public UsersController(UserContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    private static bool IsValidPassword(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Length > MinPasswordLength &&
               Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]") && 
               password.Any(char.IsDigit);
    } 

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(UserModel userRequest)
    {
        if (await _context.Users.AnyAsync(i => i.Username == userRequest.Username))
        {
            return Conflict("Username is already used!");
        }

        if (await _context.Users.AnyAsync(i => i.Email == userRequest.Email))
        {
            return Conflict("Email is already used!");
        }

        if (!IsValidPassword(userRequest.Password))
        {
            return ValidationProblem(
                $"Password is invalid! it should be at least {MinPasswordLength}, have special character and digit!");
        }

        userRequest.CreatedAt = DateTime.UtcNow;
        userRequest.UpdatedAt = DateTime.UtcNow;

        userRequest.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userRequest.Password);

        _context.Users.Add(userRequest);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginUser(UserModel userRequest)
    {
        var existingUser = await _context.Users
            .Where(user => user.Username == userRequest.Username || user.Email == userRequest.Email)
            .FirstOrDefaultAsync();
        if (existingUser is null)
        {
            return NotFound("That username or email doesn't exist!");
        }

        if (!BCrypt.Net.BCrypt.EnhancedVerify(userRequest.Password, existingUser.Password))
        {
            return Conflict("Password doesn't match!");
        }

        var token = _authService.Create(userRequest);

        return Ok(new {Token = token});
    }
}