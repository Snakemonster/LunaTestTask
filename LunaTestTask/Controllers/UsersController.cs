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
    private readonly TokenContext _tokenContext;
    private AuthService _authService;
    public const int MinPasswordLength = 8;

    public UsersController(UserContext context, AuthService authService, TokenContext tokenContext)
    {
        _context = context;
        _authService = authService;
        _tokenContext = tokenContext;
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
        var existUser = await _context.Users
            .Where(user => user.Username == userRequest.Username || user.Email == userRequest.Email)
            .FirstOrDefaultAsync();
        if (existUser is null)
        {
            return NotFound("That username or email doesn't exist!");
        }

        if (!BCrypt.Net.BCrypt.EnhancedVerify(userRequest.Password, existUser.Password))
        {
            return Conflict("Password doesn't match!");
        }

        string token;
        var existToken = await _tokenContext.GetTokenByUserId(existUser.Id);

        if (existToken is null)
        {
            token = await _authService.Create(existUser);
        }
        else if (existToken.ExpireAt < DateTime.UtcNow)
        {
            token = await _authService.Renew(existUser);
        }
        else
        {
            token = existToken.Token;
        }

        return Ok(new {Token = token});
    }
}