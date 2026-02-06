using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

/// <summary>
/// Task Status lookup table entity
/// </summary>
public class TaskStatusEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation property
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
