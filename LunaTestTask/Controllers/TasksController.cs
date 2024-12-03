using LunaTestTask.Models;
using LunaTestTask.Models.Contexts;
using LunaTestTask.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = LunaTestTask.Models.TaskStatus;

namespace LunaTestTask.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskContext _context;
    private readonly TokenContext _tokenContext;

    public TasksController(TaskContext context, TokenContext tokenContext)
    {
        _context = context;
        _tokenContext = tokenContext;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TaskModel>> CreateNewTask(TaskRequest taskRequest)
    {
        var task = taskRequest.GetTaskModel();
        if (task is null)
        {
            return BadRequest("Wrong type of Status or Priority");
        }

        var authToken = await _tokenContext.GetToken(HttpContext.Request.Headers.Authorization.ToString());

        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        task.UserId = authToken.UserId;
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new {id = task.Id}, task);
    }

    //TODO: pagination to GetAllTasks
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetAllTasks(
        [FromQuery(Name = "DueDate")] string? dueDateHeader,
        [FromQuery(Name = "Status")] string? statusHeader,
        [FromQuery(Name = "Priority")] string? priorityHeader)
    {
        var authToken = await _tokenContext.GetToken(HttpContext.Request.Headers.Authorization.ToString());

        var filter = new TaskFilter
        {
            DueDate = !string.IsNullOrEmpty(dueDateHeader) && DateTime.TryParse(dueDateHeader, out var dueDate) ? dueDate : null,
            Status = !string.IsNullOrEmpty(statusHeader) && Enum.TryParse<TaskStatus>(statusHeader, true, out var status) ? status : null,
            Priority = !string.IsNullOrEmpty(priorityHeader) && Enum.TryParse<TaskPriority>(priorityHeader, true, out var priority) ? priority : null
        };

        var tasks = await _context.GetAllTasks(authToken, filter);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TaskModel>> GetTask(Guid id)
    {
        var authToken = await _tokenContext.GetToken(HttpContext.Request.Headers.Authorization.ToString());
        if (authToken.UserId != id) return NoContent();
    
        var task = await _context.Tasks.FindAsync(id);
        if (task is null)
        {
            return NotFound("Your task doesn't exist!");
        }
        return Ok(task);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTask(Guid id, TaskRequest taskRequest)
    {
        var authToken = await _tokenContext.GetToken(HttpContext.Request.Headers.Authorization.ToString());
        if (authToken.UserId != id) return NoContent();

        if (string.IsNullOrEmpty(taskRequest.Title))
        {
            return BadRequest("Task title cannot be empty!");
        }
        var existingTask = await _context.Tasks.FindAsync(id);
        var updatedTask = taskRequest.GetTaskModel();
        if (updatedTask is null)
        {
            return BadRequest("Wrong type of Status or Priority");
        }
        if (existingTask is null)
        {
            return BadRequest("Cannot find viable task!");
        }

        existingTask.Title = updatedTask.Title;
        existingTask.Description = updatedTask.Description;
        existingTask.Priority = updatedTask.Priority;
        existingTask.Status = updatedTask.Status;
        existingTask.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var authToken = await _tokenContext.GetToken(HttpContext.Request.Headers.Authorization.ToString());
        if (authToken.UserId != id) return NoContent();

        var task = await _context.Tasks.FindAsync(id);
        if (task is null)
        {
            return BadRequest("Cannot find viable task!");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}