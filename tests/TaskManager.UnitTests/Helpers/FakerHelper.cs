using Bogus;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.UnitTests.Helpers;

/// <summary>
/// Centralized faker configurations for generating test data
/// </summary>
public static class FakerHelper
{
    /// <summary>
    /// Faker for generating TaskItem entities
    /// </summary>
    public static Faker<TaskItem> TaskItemFaker => new Faker<TaskItem>()
        .RuleFor(t => t.Id, f => f.Random.Guid())
        .RuleFor(t => t.Title, f => f.Lorem.Sentence(3, 5))
        .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
        .RuleFor(t => t.StatusId, f => f.PickRandom(TaskLookupIds.TodoStatusId, TaskLookupIds.InProgressStatusId, TaskLookupIds.DoneStatusId))
        .RuleFor(t => t.PriorityId, f => f.PickRandom(TaskLookupIds.LowPriorityId, TaskLookupIds.MediumPriorityId, TaskLookupIds.HighPriorityId))
        .RuleFor(t => t.DueDate, f => f.Date.Future())
        .RuleFor(t => t.IsCompleted, f => f.Random.Bool())
        .RuleFor(t => t.UserId, f => f.Random.Guid().ToString())
        .RuleFor(t => t.CreatedAt, f => f.Date.Past())
        .RuleFor(t => t.UpdatedAt, f => f.Date.Recent());

    /// <summary>
    /// Faker for generating CreateTaskDto objects
    /// </summary>
    public static Faker<CreateTaskDto> CreateTaskDtoFaker => new Faker<CreateTaskDto>()
        .RuleFor(t => t.Title, f => f.Lorem.Sentence(3, 5))
        .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
        .RuleFor(t => t.Priority, f => f.PickRandom<TaskPriority>())
        .RuleFor(t => t.DueDate, f => f.Date.Future());

    /// <summary>
    /// Faker for generating UpdateTaskDto objects
    /// </summary>
    public static Faker<UpdateTaskDto> UpdateTaskDtoFaker => new Faker<UpdateTaskDto>()
        .RuleFor(t => t.Title, f => f.Lorem.Sentence(3, 5))
        .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
        .RuleFor(t => t.Status, f => f.PickRandom(new TaskStatus?[] { TaskStatus.Todo, TaskStatus.InProgress, TaskStatus.Done, null }))
        .RuleFor(t => t.Priority, f => f.PickRandom(new TaskPriority?[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High, null }))
        .RuleFor(t => t.DueDate, f => f.Date.Future())
        .RuleFor(t => t.IsCompleted, f => f.Random.Bool());

    /// <summary>
    /// Faker for generating TaskDto objects
    /// </summary>
    public static Faker<TaskDto> TaskDtoFaker => new Faker<TaskDto>()
        .RuleFor(t => t.Id, f => f.Random.Guid())
        .RuleFor(t => t.Title, f => f.Lorem.Sentence(3, 5))
        .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
        .RuleFor(t => t.Status, f => f.PickRandom<TaskStatus>())
        .RuleFor(t => t.Priority, f => f.PickRandom<TaskPriority>())
        .RuleFor(t => t.DueDate, f => f.Date.Future())
        .RuleFor(t => t.IsCompleted, f => f.Random.Bool())
        .RuleFor(t => t.CreatedAt, f => f.Date.Past())
        .RuleFor(t => t.UpdatedAt, f => f.Date.Recent());

    /// <summary>
    /// Generate a valid user ID
    /// </summary>
    public static string GenerateUserId() => Guid.NewGuid().ToString();

    /// <summary>
    /// Generate a list of task items
    /// </summary>
    public static List<TaskItem> GenerateTaskItems(int count, string? userId = null)
    {
        var faker = TaskItemFaker;
        if (!string.IsNullOrEmpty(userId))
        {
            faker = faker.RuleFor(t => t.UserId, userId);
        }
        return faker.Generate(count);
    }

    /// <summary>
    /// Generate a single task item
    /// </summary>
    public static TaskItem GenerateTaskItem(string? userId = null)
    {
        return GenerateTaskItems(1, userId).First();
    }
}
