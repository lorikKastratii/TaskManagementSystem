using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
    {
        _logger.LogInformation("GET api/tasks - Fetching all tasks");
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTaskById(Guid id)
    {
        _logger.LogInformation("GET api/tasks/{TaskId} - Fetching task", id);
        var task = await _taskService.GetTaskByIdAsync(id);
        
        if (task == null)
        {
            _logger.LogWarning("GET api/tasks/{TaskId} - Task not found", id);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        _logger.LogInformation("POST api/tasks - Creating new task: {Title}", createTaskDto.Title);
        
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("POST api/tasks - Invalid model state");
            return BadRequest(ModelState);
        }

        var task = await _taskService.CreateTaskAsync(createTaskDto);
        _logger.LogInformation("POST api/tasks - Task created with ID: {TaskId}", task.Id);
        
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        _logger.LogInformation("PUT api/tasks/{TaskId} - Updating task", id);
        
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("PUT api/tasks/{TaskId} - Invalid model state", id);
            return BadRequest(ModelState);
        }

        try
        {
            var task = await _taskService.UpdateTaskAsync(id, updateTaskDto);
            _logger.LogInformation("PUT api/tasks/{TaskId} - Task updated successfully", id);
            return Ok(task);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "PUT api/tasks/{TaskId} - Task not found", id);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(Guid id)
    {
        _logger.LogInformation("DELETE api/tasks/{TaskId} - Deleting task", id);
        var result = await _taskService.DeleteTaskAsync(id);
        
        if (!result)
        {
            _logger.LogWarning("DELETE api/tasks/{TaskId} - Task not found", id);
            return NotFound(new { message = $"Task with ID {id} not found" });
        }

        _logger.LogInformation("DELETE api/tasks/{TaskId} - Task deleted successfully", id);
        return NoContent();
    }
}
