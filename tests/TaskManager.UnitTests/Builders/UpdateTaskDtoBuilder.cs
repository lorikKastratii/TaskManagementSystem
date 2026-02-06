using TaskManager.Application.DTOs;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskManager.UnitTests.Helpers;

namespace TaskManager.UnitTests.Builders;

/// <summary>
/// Builder pattern for creating UpdateTaskDto test objects
/// </summary>
public class UpdateTaskDtoBuilder
{
    private readonly UpdateTaskDto _dto;

    public UpdateTaskDtoBuilder()
    {
        // Start with a faker-generated object
        _dto = FakerHelper.UpdateTaskDtoFaker.Generate();
    }

    public UpdateTaskDtoBuilder WithTitle(string title)
    {
        _dto.Title = title;
        return this;
    }

    public UpdateTaskDtoBuilder WithDescription(string description)
    {
        _dto.Description = description;
        return this;
    }

    public UpdateTaskDtoBuilder WithStatus(TaskStatus? status)
    {
        _dto.Status = status;
        return this;
    }

    public UpdateTaskDtoBuilder WithPriority(TaskPriority? priority)
    {
        _dto.Priority = priority;
        return this;
    }

    public UpdateTaskDtoBuilder WithDueDate(DateTime? dueDate)
    {
        _dto.DueDate = dueDate;
        return this;
    }

    public UpdateTaskDtoBuilder WithIsCompleted(bool? isCompleted)
    {
        _dto.IsCompleted = isCompleted;
        return this;
    }

    public UpdateTaskDtoBuilder AsTodoTask()
    {
        _dto.Status = TaskStatus.Todo;
        _dto.IsCompleted = false;
        return this;
    }

    public UpdateTaskDtoBuilder AsInProgressTask()
    {
        _dto.Status = TaskStatus.InProgress;
        _dto.IsCompleted = false;
        return this;
    }

    public UpdateTaskDtoBuilder AsCompletedTask()
    {
        _dto.Status = TaskStatus.Done;
        _dto.IsCompleted = true;
        return this;
    }

    public UpdateTaskDtoBuilder WithHighPriority()
    {
        _dto.Priority = TaskPriority.High;
        return this;
    }

    public UpdateTaskDtoBuilder WithLowPriority()
    {
        _dto.Priority = TaskPriority.Low;
        return this;
    }

    public UpdateTaskDtoBuilder WithMediumPriority()
    {
        _dto.Priority = TaskPriority.Medium;
        return this;
    }

    public UpdateTaskDto Build()
    {
        return _dto;
    }

    /// <summary>
    /// Create a default UpdateTaskDto builder
    /// </summary>
    public static UpdateTaskDtoBuilder Default() => new();
}
