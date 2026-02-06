using TaskManager.Domain.Common;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;

namespace TaskManager.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Foreign Keys
    public Guid StatusId { get; set; }
    public Guid PriorityId { get; set; }
    
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }

    public string UserId { get; set; } = string.Empty;
    
    // Navigation properties
    public TaskStatusEntity? Status { get; set; }
    public TaskPriorityEntity? Priority { get; set; }
    
    // Helper properties for backward compatibility with enum usage
    public TaskStatus StatusEnum => Status?.Name switch
    {
        "Todo" => TaskStatus.Todo,
        "InProgress" => TaskStatus.InProgress,
        "Done" => TaskStatus.Done,
        _ => TaskStatus.Todo
    };
    
    public TaskPriority PriorityEnum => Priority?.Name switch
    {
        "Low" => TaskPriority.Low,
        "Medium" => TaskPriority.Medium,
        "High" => TaskPriority.High,
        _ => TaskPriority.Medium
    };
}
