using Microsoft.EntityFrameworkCore;

namespace LunaTestTask.Models.Contexts;

public class UserContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserModel>().ToTable("User");
    }
}
