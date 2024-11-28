using Microsoft.EntityFrameworkCore;

namespace LunaTestTask.Models.Contexts;

public class UserContext : DbContext
{
    public DbSet<string> Id { get; set; }
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }
}
