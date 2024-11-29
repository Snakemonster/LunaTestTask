using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LunaTestTask.Models;
using Microsoft.IdentityModel.Tokens;

namespace LunaTestTask.Services;

public class AuthService
{
    public string Create(UserModel user)
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