using Microsoft.AspNetCore.Mvc;

namespace LunaTestTask.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateNewTask()
    {
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(string id)
    {
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(string id)
    {
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(string id)
    {
        return NoContent();
    }
}