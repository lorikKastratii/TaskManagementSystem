namespace TaskManager.Application.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StatusId { get; set; } = 0; // Default to Todo
    public int PriorityId { get; set; } = 1; // Default to Medium
    public DateTime? DueDate { get; set; }
}
