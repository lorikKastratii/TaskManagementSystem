namespace TaskManager.Application.DTOs;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? StatusId { get; set; }
    public int? PriorityId { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? IsCompleted { get; set; }
}
