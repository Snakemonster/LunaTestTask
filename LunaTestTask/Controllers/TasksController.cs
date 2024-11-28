using LunaTestTask.Models;
using LunaTestTask.Models.Contexts;
using LunaTestTask.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LunaTestTask.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskContext _taskContext;

    public TasksController(TaskContext context)
    {
        _taskContext = context;
    }

    [HttpPost]
    public async Task<ActionResult<TaskModel>> CreateNewTask(TaskRequest taskRequest)
    {
        var task = taskRequest.GetTaskModel();
        if (task is null) return NoContent();

        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        _taskContext.Tasks.Add(task);
        await _taskContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new {id = task.Id}, task);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetAllTasks()
    {
        var users = await _taskContext.Tasks.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskModel>> GetTask(Guid id)
    {
        var task = await _taskContext.Tasks.FindAsync(id);
        if (task is null) return NotFound();
        return Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id)
    {
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        return NoContent();
    }
}