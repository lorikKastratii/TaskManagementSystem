using Microsoft.AspNetCore.Identity;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
