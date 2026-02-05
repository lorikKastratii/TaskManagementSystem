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

    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        _logger.LogInformation("Fetching all tasks");
        var tasks = await _taskRepository.GetAllAsync();
        _logger.LogInformation("Retrieved {Count} tasks", tasks.Count());
        return _mapper.Map<IEnumerable<TaskDto>>(tasks);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching task with ID: {TaskId}", id);
        var task = await _taskRepository.GetByIdAsync(id);
        
        if (task == null)
        {
            _logger.LogWarning("Task with ID: {TaskId} not found", id);
            return null;
        }

        _logger.LogInformation("Task with ID: {TaskId} retrieved successfully", id);
        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        _logger.LogInformation("Creating new task with title: {Title}", createTaskDto.Title);
        
        var task = _mapper.Map<TaskItem>(createTaskDto);
        task.Id = Guid.NewGuid();
        task.Status = TaskStatus.Todo;
        task.IsCompleted = false;
        task.CreatedAt = DateTime.UtcNow;

        var createdTask = await _taskRepository.CreateAsync(task);
        _logger.LogInformation("Task created successfully with ID: {TaskId}", createdTask.Id);
        
        return _mapper.Map<TaskDto>(createdTask);
    }

    public async Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);
        
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            _logger.LogError("Cannot update task. Task with ID: {TaskId} not found", id);
            throw new KeyNotFoundException($"Task with ID {id} not found");
        }

        _mapper.Map(updateTaskDto, task);
        task.UpdatedAt = DateTime.UtcNow;

        var updatedTask = await _taskRepository.UpdateAsync(task);
        _logger.LogInformation("Task with ID: {TaskId} updated successfully", id);
        
        return _mapper.Map<TaskDto>(updatedTask);
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);
        
        var result = await _taskRepository.DeleteAsync(id);
        
        if (result)
        {
            _logger.LogInformation("Task with ID: {TaskId} deleted successfully", id);
        }
        else
        {
            _logger.LogWarning("Failed to delete task. Task with ID: {TaskId} not found", id);
        }
        
        return result;
    }
}
