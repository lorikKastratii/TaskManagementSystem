using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository taskRepository, IMapper mapper, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync(string userId)
    {
        _logger.LogInformation("Fetching all tasks for user {UserId}", userId);
        var tasks = await _taskRepository.GetAllAsync();
        var userTasks = tasks.Where(t => t.UserId == userId);
        _logger.LogInformation("Retrieved {Count} tasks for user {UserId}", userTasks.Count(), userId);
        return _mapper.Map<IEnumerable<TaskDto>>(userTasks);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(Guid id, string userId)
    {
        _logger.LogInformation("Fetching task with ID: {TaskId} for user {UserId}", id, userId);
        var task = await _taskRepository.GetByIdAsync(id);

        if (task == null || task.UserId != userId)
        {
            _logger.LogWarning("Task with ID: {TaskId} not found for user {UserId}", id, userId);
            return null;
        }

        _logger.LogInformation("Task with ID: {TaskId} retrieved successfully for user {UserId}", id, userId);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, string userId)
    {
        _logger.LogInformation("Creating new task with title: {Title} for user {UserId}", createTaskDto.Title, userId);

        var task = _mapper.Map<TaskItem>(createTaskDto);
        task.Id = Guid.NewGuid();
        task.StatusId = TaskLookupIds.TodoStatusId;
        task.IsCompleted = false;
        task.CreatedAt = DateTime.UtcNow;
        task.UserId = userId;

        var createdTask = await _taskRepository.CreateAsync(task);
        _logger.LogInformation("Task created successfully with ID: {TaskId} for user {UserId}", createdTask.Id, userId);

        return _mapper.Map<TaskDto>(createdTask);
    }

    public async Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto, string userId)
    {
        _logger.LogInformation("Updating task with ID: {TaskId} for user {UserId}", id, userId);

        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            _logger.LogError("Cannot update task. Task with ID: {TaskId} not found", id);
            throw new KeyNotFoundException($"Task with ID {id} not found");
        }

        if (task.UserId != userId)
        {
            _logger.LogError("Cannot update task. User {UserId} is not authorized to update task {TaskId}", userId, id);
            throw new UnauthorizedAccessException($"User is not authorized to update this task");
        }

        _mapper.Map(updateTaskDto, task);
        task.UpdatedAt = DateTime.UtcNow;

        var updatedTask = await _taskRepository.UpdateAsync(task);
        _logger.LogInformation("Task with ID: {TaskId} updated successfully for user {UserId}", id, userId);

        return _mapper.Map<TaskDto>(updatedTask);
    }

    public async Task<bool> DeleteTaskAsync(Guid id, string userId)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId} for user {UserId}", id, userId);

        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            _logger.LogWarning("Failed to delete task. Task with ID: {TaskId} not found", id);
            return false;
        }

        if (task.UserId != userId)
        {
            _logger.LogError("Cannot delete task. User {UserId} is not authorized to delete task {TaskId}", userId, id);
            throw new UnauthorizedAccessException($"User is not authorized to delete this task");
        }

        var result = await _taskRepository.DeleteAsync(id);

        if (result)
        {
            _logger.LogInformation("Task with ID: {TaskId} deleted successfully for user {UserId}", id, userId);
        }
        else
        {
            _logger.LogWarning("Failed to delete task with ID: {TaskId} for user {UserId}", id, userId);
        }

        return result;
    }
}
