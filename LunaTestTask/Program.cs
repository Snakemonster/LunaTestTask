using System.Text;
using LunaTestTask.Models.Contexts;
using LunaTestTask.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TokenContext = LunaTestTask.Models.Contexts.TokenContext;

namespace LunaTestTask;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var secretKey = Configuration.TokenSecretKey = builder.Configuration.GetSection("TokenSecretKey").Value;
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentException("Secret key is empty or null!");
        }

        Configuration.TokenSecretKey = secretKey;
        builder.Services.AddControllers();
        builder.Services.AddDbContextPool<UserContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("LunaTestDB")));
        builder.Services.AddDbContextPool<TaskContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("LunaTestDB")));
        builder.Services.AddDbContextPool<TokenContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("LunaTestDB")));
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.TokenSecretKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var tokenContext = context.HttpContext.RequestServices.GetRequiredService<TokenContext>();
                    var token = (JsonWebToken)context.SecurityToken;

                    if (token == null || string.IsNullOrWhiteSpace(token.EncodedToken))
                    {
                        context.Fail("Invalid token");
                        return;
                    }

                    var tokenExist = await tokenContext.Tokens.AnyAsync(tok => tok.Token == token.EncodedToken);
                    if (!tokenExist)
                    {
                        context.Fail("Token is not recognized!");
                    }
                }
            };
        });
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}