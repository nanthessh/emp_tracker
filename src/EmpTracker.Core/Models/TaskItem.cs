namespace EmpTracker.Core.Models;

public class TaskItem
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AssignedTo { get; set; }
    public string AssigneeName { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Pending";
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
