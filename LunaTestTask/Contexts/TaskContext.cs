using Microsoft.EntityFrameworkCore;

namespace LunaTestTask.Models.Contexts;

public class TaskContext : DbContext
{
    public DbSet<TaskModel> Tasks { get; set; }
    
    public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TaskModel>().ToTable("Task");

        modelBuilder.Entity<TaskModel>().Property(i => i.Status).HasConversion<string>();
        modelBuilder.Entity<TaskModel>().Property(i => i.Priority).HasConversion<string>();
    }
}