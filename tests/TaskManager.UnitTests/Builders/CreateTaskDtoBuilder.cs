using TaskManager.Application.DTOs;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskManager.UnitTests.Helpers;

namespace TaskManager.UnitTests.Builders;

/// <summary>
/// Builder pattern for creating CreateTaskDto test objects
/// </summary>
public class CreateTaskDtoBuilder
{
    private readonly CreateTaskDto _dto;

    public CreateTaskDtoBuilder()
    {
        // Start with a faker-generated object
        _dto = FakerHelper.CreateTaskDtoFaker.Generate();
    }

    public CreateTaskDtoBuilder WithTitle(string title)
    {
        _dto.Title = title;
        return this;
    }

    public CreateTaskDtoBuilder WithDescription(string description)
    {
        _dto.Description = description;
        return this;
    }

    public CreateTaskDtoBuilder WithPriority(TaskPriority priority)
    {
        _dto.Priority = priority;
        return this;
    }

    public CreateTaskDtoBuilder WithDueDate(DateTime? dueDate)
    {
        _dto.DueDate = dueDate;
        return this;
    }

    public CreateTaskDtoBuilder WithHighPriority()
    {
        _dto.Priority = TaskPriority.High;
        return this;
    }

    public CreateTaskDtoBuilder WithLowPriority()
    {
        _dto.Priority = TaskPriority.Low;
        return this;
    }

    public CreateTaskDtoBuilder WithMediumPriority()
    {
        _dto.Priority = TaskPriority.Medium;
        return this;
    }

    public CreateTaskDtoBuilder WithNoDueDate()
    {
        _dto.DueDate = null;
        return this;
    }

    public CreateTaskDto Build()
    {
        return _dto;
    }

    /// <summary>
    /// Create a default CreateTaskDto builder
    /// </summary>
    public static CreateTaskDtoBuilder Default() => new();
}
