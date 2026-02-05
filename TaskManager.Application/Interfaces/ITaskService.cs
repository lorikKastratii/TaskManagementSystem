using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllTasksAsync(string userId);
    Task<TaskDto?> GetTaskByIdAsync(Guid id, string userId);
    Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, string userId);
    Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto, string userId);
    Task<bool> DeleteTaskAsync(Guid id, string userId);
}
