using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User ID not found in token");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
    {
        var userId = GetUserId();
        _logger.LogInformation("GET api/tasks - Fetching all tasks for user {UserId}", userId);
        var tasks = await _taskService.GetAllTasksAsync(userId);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTaskById(Guid id)
    {
        var userId = GetUserId();
        _logger.LogInformation("GET api/tasks/{TaskId} - Fetching task for user {UserId}", id, userId);
        var task = await _taskService.GetTaskByIdAsync(id, userId);

        if (task == null)
        {
            _logger.LogWarning("GET api/tasks/{TaskId} - Task not found for user {UserId}", id, userId);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        var userId = GetUserId();
        _logger.LogInformation("POST api/tasks - Creating new task: {Title} for user {UserId}", createTaskDto.Title, userId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("POST api/tasks - Invalid model state for user {UserId}", userId);
            return BadRequest(ModelState);
        }

        var task = await _taskService.CreateTaskAsync(createTaskDto, userId);
        _logger.LogInformation("POST api/tasks - Task created with ID: {TaskId} for user {UserId}", task.Id, userId);

        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        var userId = GetUserId();
        _logger.LogInformation("PUT api/tasks/{TaskId} - Updating task for user {UserId}", id, userId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("PUT api/tasks/{TaskId} - Invalid model state for user {UserId}", id, userId);
            return BadRequest(ModelState);
        }

        try
        {
            var task = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);
            _logger.LogInformation("PUT api/tasks/{TaskId} - Task updated successfully for user {UserId}", id, userId);
            return Ok(task);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "PUT api/tasks/{TaskId} - Task not found for user {UserId}", id, userId);
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "PUT api/tasks/{TaskId} - Unauthorized access for user {UserId}", id, userId);
            return Forbid();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(Guid id)
    {
        var userId = GetUserId();
        _logger.LogInformation("DELETE api/tasks/{TaskId} - Deleting task for user {UserId}", id, userId);

        try
        {
            var result = await _taskService.DeleteTaskAsync(id, userId);

            if (!result)
            {
                _logger.LogWarning("DELETE api/tasks/{TaskId} - Task not found for user {UserId}", id, userId);
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            _logger.LogInformation("DELETE api/tasks/{TaskId} - Task deleted successfully for user {UserId}", id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "DELETE api/tasks/{TaskId} - Unauthorized access for user {UserId}", id, userId);
            return Forbid();
        }
    }
}
