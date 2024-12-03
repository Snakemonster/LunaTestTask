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

    public async Task<bool> CheckId(Guid id)
    {
        return await Tasks.FindAsync(id) is not null;
    }

    public async Task CreateTask(Guid userId, TaskModel outTaskCredentials)
    {
        outTaskCredentials.CreatedAt = DateTime.UtcNow;
        outTaskCredentials.UpdatedAt = DateTime.UtcNow;
        outTaskCredentials.UserId = userId;
        Tasks.Add(outTaskCredentials);
        await SaveChangesAsync();
    }
    public async Task<IEnumerable<TaskModel>> GetAllTasks(Guid userId, TaskFilter filter)
    {
        var tasks = Tasks.Where(token => token.UserId == userId).AsEnumerable();
        return Filter(tasks, filter);
    }

    public async Task<TaskModel> GetTask(Guid userId, Guid tokenId)
    {
        var oneTask = await Tasks.Where(task => task.Id == tokenId).FirstOrDefaultAsync();
        return userId != oneTask.UserId ? null! : oneTask;
    }

    public async Task UpdateTask(Guid userId, Guid taskId, TaskModel newTask)
    {
        var existingTask = await Tasks.FindAsync(taskId);
        if (userId != existingTask.UserId) return;
        existingTask.Title = newTask.Title;
        existingTask.Description = newTask.Description;
        existingTask.Priority = newTask.Priority;
        existingTask.Status = newTask.Status;
        existingTask.UpdatedAt = DateTime.UtcNow;
        await SaveChangesAsync();
    }
    
    
    public async Task DeleteTask(Guid userId, Guid id)
    {
        var task = await Tasks.FindAsync(id);
        if (userId != task.UserId) return;
        Tasks.Remove(task);
        await SaveChangesAsync();
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