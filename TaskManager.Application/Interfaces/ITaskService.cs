using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllTasksAsync();
    Task<TaskDto?> GetTaskByIdAsync(Guid id);
    Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task<TaskDto> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto);
    Task<bool> DeleteTaskAsync(Guid id);
}
