using TaskManager.Domain.Entities;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskManager.UnitTests.Helpers;

namespace TaskManager.UnitTests.Builders;

/// <summary>
/// Builder pattern for creating TaskItem test objects
/// </summary>
public class TaskItemBuilder
{
    private readonly TaskItem _taskItem;

    public TaskItemBuilder()
    {
        // Start with a faker-generated object
        _taskItem = FakerHelper.TaskItemFaker.Generate();
    }

    public TaskItemBuilder WithId(Guid id)
    {
        _taskItem.Id = id;
        return this;
    }

    public TaskItemBuilder WithTitle(string title)
    {
        _taskItem.Title = title;
        return this;
    }

    public TaskItemBuilder WithDescription(string description)
    {
        _taskItem.Description = description;
        return this;
    }

    public TaskItemBuilder WithStatus(TaskStatus status)
    {
        _taskItem.StatusId = TaskLookupIds.GetStatusId(status);
        return this;
    }

    public TaskItemBuilder WithPriority(TaskPriority priority)
    {
        _taskItem.PriorityId = TaskLookupIds.GetPriorityId(priority);
        return this;
    }

    public TaskItemBuilder WithDueDate(DateTime? dueDate)
    {
        _taskItem.DueDate = dueDate;
        return this;
    }

    public TaskItemBuilder WithIsCompleted(bool isCompleted)
    {
        _taskItem.IsCompleted = isCompleted;
        return this;
    }

    public TaskItemBuilder WithUserId(string userId)
    {
        _taskItem.UserId = userId;
        return this;
    }

    public TaskItemBuilder WithCreatedAt(DateTime createdAt)
    {
        _taskItem.CreatedAt = createdAt;
        return this;
    }

    public TaskItemBuilder WithUpdatedAt(DateTime? updatedAt)
    {
        _taskItem.UpdatedAt = updatedAt;
        return this;
    }

    public TaskItemBuilder AsTodoTask()
    {
        _taskItem.StatusId = TaskLookupIds.TodoStatusId;
        _taskItem.IsCompleted = false;
        return this;
    }

    public TaskItemBuilder AsInProgressTask()
    {
        _taskItem.StatusId = TaskLookupIds.InProgressStatusId;
        _taskItem.IsCompleted = false;
        return this;
    }

    public TaskItemBuilder AsCompletedTask()
    {
        _taskItem.StatusId = TaskLookupIds.DoneStatusId;
        _taskItem.IsCompleted = true;
        return this;
    }

    public TaskItemBuilder WithHighPriority()
    {
        _taskItem.PriorityId = TaskLookupIds.HighPriorityId;
        return this;
    }

    public TaskItemBuilder WithLowPriority()
    {
        _taskItem.PriorityId = TaskLookupIds.LowPriorityId;
        return this;
    }

    public TaskItemBuilder WithMediumPriority()
    {
        _taskItem.PriorityId = TaskLookupIds.MediumPriorityId;
        return this;
    }

    public TaskItem Build()
    {
        return _taskItem;
    }

    /// <summary>
    /// Create a default TaskItem builder
    /// </summary>
    public static TaskItemBuilder Default() => new();

    /// <summary>
    /// Create a builder with a specific user ID
    /// </summary>
    public static TaskItemBuilder ForUser(string userId) => new TaskItemBuilder().WithUserId(userId);
}
