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
        if (task.Item1 is null) return BadRequest($"{task.Item2}");

        var userId = await _tokenContext.GetTokenUserId(HttpContext.Request.Headers.Authorization.ToString());

        await _context.CreateTask(userId, task.Item1);
        return CreatedAtAction(nameof(GetTask), new {id = task.Item1.Id}, task);
    }

    //TODO: pagination to GetAllTasks
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TaskModel>>> GetAllTasks(
        [FromQuery(Name = "DueDate")] string? dueDateHeader,
        [FromQuery(Name = "Status")] string? statusHeader,
        [FromQuery(Name = "Priority")] string? priorityHeader)
    {
        var userId = await _tokenContext.GetTokenUserId(HttpContext.Request.Headers.Authorization.ToString());

        var filter = new TaskFilter
        {
            DueDate = !string.IsNullOrEmpty(dueDateHeader) && DateTime.TryParse(dueDateHeader, out var dueDate) ? dueDate : null,
            Status = !string.IsNullOrEmpty(statusHeader) && Enum.TryParse<TaskStatus>(statusHeader, true, out var status) ? status : null,
            Priority = !string.IsNullOrEmpty(priorityHeader) && Enum.TryParse<TaskPriority>(priorityHeader, true, out var priority) ? priority : null
        };

        var tasks = await _context.GetAllTasks(userId, filter);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TaskModel>> GetTask(Guid id)
    {
        if (await _context.CheckId(id)) return NotFound("Cannot found task with that id");
        var userId = await _tokenContext.GetTokenUserId(HttpContext.Request.Headers.Authorization.ToString());
        var task = await _context.GetTask(userId, id);
        return Ok(task);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTask(Guid id, TaskRequest taskRequest)
    {
        if (await _context.CheckId(id)) return NotFound("Cannot found task with that id");
        var userId = await _tokenContext.GetTokenUserId(HttpContext.Request.Headers.Authorization.ToString());

        var updatedTask = taskRequest.GetTaskModel();
        if (updatedTask.Item1 is null) return BadRequest($"{updatedTask.Item2}");

        await _context.UpdateTask(userId, id, updatedTask.Item1);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        if (await _context.CheckId(id)) return NotFound("Cannot found task with that id");
        var userId = await _tokenContext.GetTokenUserId(HttpContext.Request.Headers.Authorization.ToString());
        await _context.DeleteTask(userId, id);
        return NoContent();
    }
}