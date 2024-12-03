using System.Text;

namespace LunaTestTask.Models.Requests;

public class TaskRequest
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }

    public (TaskModel?, string) GetTaskModel()
    {
        var errors = new StringBuilder();
        if (string.IsNullOrEmpty(Title)) errors.Append("Title cannot be empty\n");

        if (DueDate < DateTime.UtcNow) errors.Append("Task cannot have dueDate in past\n");

        TaskStatus newStatus;
        if (!Enum.TryParse(Status, out newStatus)) errors.Append("Unknown type of Status (should be Pending, InProgress or Completed)\n");

        TaskPriority newTaskPriority;
        if (!Enum.TryParse(Priority, out newTaskPriority)) errors.Append("Unknown type of Priority (should be Low, Medium or High)");

        if (errors.Length > 0) return (null, errors.ToString());
        var newTaskModel = new TaskModel
        {
            Title = Title,
            Description = Description,
            DueDate = DueDate,
            Status = newStatus,
            Priority = newTaskPriority
        };
        return (newTaskModel, "Ok");
    }
}