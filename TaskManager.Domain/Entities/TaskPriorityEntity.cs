using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

/// <summary>
/// Task Priority lookup table entity
/// </summary>
public class TaskPriorityEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation property
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
