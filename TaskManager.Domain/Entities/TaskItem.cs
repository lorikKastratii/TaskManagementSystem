using TaskManager.Domain.Common;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;

namespace TaskManager.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }

    public string UserId { get; set; } = string.Empty;
}
