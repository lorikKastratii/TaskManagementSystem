using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;

namespace TaskManager.Application.DTOs;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? IsCompleted { get; set; }
}
