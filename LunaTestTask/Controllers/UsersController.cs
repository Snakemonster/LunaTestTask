using System.Text.RegularExpressions;
using LunaTestTask.Models;
using LunaTestTask.Models.Contexts;
using LunaTestTask.Services;
using Microsoft.AspNetCore.Mvc;

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
        var userCheck = await _context.ValidUniqueUser(userRequest);
        if (!string.IsNullOrEmpty(userCheck)) return BadRequest(userCheck);

        if (!IsValidPassword(userRequest.Password)) return ValidationProblem(
            $"Password is invalid! it should be at least {MinPasswordLength}, have special character and digit!");

        userRequest.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userRequest.Password);

        await _context.AddUser(userRequest);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginUser(UserModel userRequest)
    {
        var existUser = await _context.GetUser(userRequest);
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