using TaskPriority = TaskManager.Domain.Enums.TaskPriority;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Domain.Entities;

/// <summary>
/// Helper class to map between enums and database entity IDs
/// </summary>
public static class TaskLookupIds
{
    // TaskStatus IDs
    public static readonly Guid TodoStatusId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid InProgressStatusId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid DoneStatusId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    // TaskPriority IDs
    public static readonly Guid LowPriorityId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid MediumPriorityId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    public static readonly Guid HighPriorityId = Guid.Parse("66666666-6666-6666-6666-666666666666");

    /// <summary>
    /// Get TaskStatus entity ID from enum
    /// </summary>
    public static Guid GetStatusId(TaskStatus status)
    {
        return status switch
        {
            TaskStatus.Todo => TodoStatusId,
            TaskStatus.InProgress => InProgressStatusId,
            TaskStatus.Done => DoneStatusId,
            _ => TodoStatusId
        };
    }

    /// <summary>
    /// Get TaskPriority entity ID from enum
    /// </summary>
    public static Guid GetPriorityId(TaskPriority priority)
    {
        return priority switch
        {
            TaskPriority.Low => LowPriorityId,
            TaskPriority.Medium => MediumPriorityId,
            TaskPriority.High => HighPriorityId,
            _ => MediumPriorityId
        };
    }

    /// <summary>
    /// Get TaskStatus enum from entity ID
    /// </summary>
    public static TaskStatus GetStatusEnum(Guid statusId)
    {
        if (statusId == TodoStatusId) return TaskStatus.Todo;
        if (statusId == InProgressStatusId) return TaskStatus.InProgress;
        if (statusId == DoneStatusId) return TaskStatus.Done;
        return TaskStatus.Todo;
    }

    /// <summary>
    /// Get TaskPriority enum from entity ID
    /// </summary>
    public static TaskPriority GetPriorityEnum(Guid priorityId)
    {
        if (priorityId == LowPriorityId) return TaskPriority.Low;
        if (priorityId == MediumPriorityId) return TaskPriority.Medium;
        if (priorityId == HighPriorityId) return TaskPriority.High;
        return TaskPriority.Medium;
    }

    /// <summary>
    /// Get TaskStatus entity ID from integer value (0=Todo, 1=InProgress, 2=Done)
    /// </summary>
    public static Guid GetStatusIdFromInt(int statusInt)
    {
        return statusInt switch
        {
            0 => TodoStatusId,           // Todo
            1 => InProgressStatusId,     // InProgress
            2 => DoneStatusId,           // Done
            _ => TodoStatusId
        };
    }

    /// <summary>
    /// Get TaskPriority entity ID from integer value (0=Low, 1=Medium, 2=High)
    /// </summary>
    public static Guid GetPriorityIdFromInt(int priorityInt)
    {
        return priorityInt switch
        {
            0 => LowPriorityId,          // Low
            1 => MediumPriorityId,       // Medium
            2 => HighPriorityId,         // High
            _ => MediumPriorityId
        };
    }
}
