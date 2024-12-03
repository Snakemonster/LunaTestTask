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

    public async Task<IEnumerable<TaskModel>> GetAllTasks(TokenModel authToken, TaskFilter filter)
    {
        var tasks = Tasks.Where(token => token.UserId == authToken.UserId).AsEnumerable();
        return Filter(tasks, filter);
    }
    
    private IEnumerable<TaskModel> Filter(IEnumerable<TaskModel> tasks, TaskFilter filter)
    {
        if (filter.DueDate.HasValue)
        {
            tasks = tasks.Where(task => task.DueDate.HasValue && task.DueDate.Value.Date == filter.DueDate.Value.Date);
        }

        if (filter.Status.HasValue)
        {
            tasks = tasks.Where(task => task.Status == filter.Status.Value);
        }

        if (filter.Priority.HasValue)
        {
            tasks = tasks.Where(task => task.Priority == filter.Priority.Value);
        }

        return tasks;
    }
}

public class TaskFilter
{
    public DateTime? DueDate { get; set; }
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
}