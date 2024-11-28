using LunaTestTask.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LunaTestTask;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddDbContextPool<UserContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("LunaTestDB")));
        builder.Services.AddDbContextPool<TaskContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("LunaTestDB")));
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