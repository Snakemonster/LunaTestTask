using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LunaTestTask.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TokenContext = LunaTestTask.Models.Contexts.TokenContext;

namespace LunaTestTask.Services;

public class AuthService
{
    private readonly TokenContext _tokenContext;

    public AuthService(TokenContext tokenContext)
    {
        _tokenContext = tokenContext;
    }
    public async Task<string> Create(UserModel user)
    {
        var tokenString = CreateNewToken(user);
        await _tokenContext.Tokens.AddAsync(new TokenModel
        {
            Token = tokenString,
            UserId = user.Id,
            ExpireAt = DateTime.UtcNow.AddDays(30)
        });
        await _tokenContext.SaveChangesAsync();
        return tokenString;
    }

    public async Task<string> Renew(UserModel user)
    {
        var oldToken = await _tokenContext.Tokens.Where(tok => tok.UserId == user.Id).FirstOrDefaultAsync();
        _tokenContext.Tokens.Remove(oldToken);

        var newTokenString = CreateNewToken(user);
        var tokenString = CreateNewToken(user);
        await _tokenContext.Tokens.AddAsync(new TokenModel
        {
            Token = tokenString,
            UserId = user.Id,
            ExpireAt = DateTime.UtcNow.AddDays(30)
        });
        await _tokenContext.SaveChangesAsync();
        return newTokenString;
    }

    private string CreateNewToken(UserModel user)
    {
        var handler = new JwtSecurityTokenHandler();

        var privateKey = Encoding.UTF8.GetBytes(Configuration.TokenSecretKey);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(privateKey),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddDays(30),
            Subject = GenClaims(user)
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenClaims(UserModel user)
    {
        var ci = new ClaimsIdentity();
        
        ci.AddClaim(new Claim("id", user.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Name, user.Username));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        return ci;
    }
}