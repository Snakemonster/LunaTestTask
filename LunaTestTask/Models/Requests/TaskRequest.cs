namespace LunaTestTask.Models.Requests;

public class TaskRequest
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }

    public TaskModel? GetTaskModel()
    {
        TaskStatus newStatus;
        if (!Enum.TryParse(Status, out newStatus))
        {
            return null;
        }

        TaskPriority newTaskPriority;
        if (!Enum.TryParse(Priority, out newTaskPriority))
        {
            return null;
        }

        var newTaskModel = new TaskModel()
        {
            Title = Title,
            Description = Description,
            DueDate = DueDate,
            Status = newStatus,
            Priority = newTaskPriority
        };
        return newTaskModel;
    }
}