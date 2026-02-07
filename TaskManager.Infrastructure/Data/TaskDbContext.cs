using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Identity;

namespace TaskManager.Infrastructure.Data;

public class TaskDbContext : IdentityDbContext<ApplicationUser>
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<TaskStatusEntity> TaskStatuses { get; set; }
    public DbSet<TaskPriorityEntity> TaskPriorities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure TaskStatusEntity
        modelBuilder.Entity<TaskStatusEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure TaskPriorityEntity
        modelBuilder.Entity<TaskPriorityEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.DisplayOrder).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure TaskItem
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.StatusId).IsRequired();
            entity.Property(e => e.PriorityId).IsRequired();

            // Foreign key relationships
            entity.HasOne(e => e.Status)
                .WithMany(s => s.Tasks)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Priority)
                .WithMany(p => p.Tasks)
                .HasForeignKey(e => e.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<ApplicationUser>()
                .WithMany(u => u.Tasks)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed TaskStatuses
        var todoStatusId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var inProgressStatusId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var doneStatusId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<TaskStatusEntity>().HasData(
            new TaskStatusEntity
            {
                Id = todoStatusId,
                Name = "Todo",
                Description = "Task is pending and not yet started",
                CreatedAt = DateTime.UtcNow
            },
            new TaskStatusEntity
            {
                Id = inProgressStatusId,
                Name = "InProgress",
                Description = "Task is currently being worked on",
                CreatedAt = DateTime.UtcNow
            },
            new TaskStatusEntity
            {
                Id = doneStatusId,
                Name = "Done",
                Description = "Task has been completed",
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed TaskPriorities
        var lowPriorityId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var mediumPriorityId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var highPriorityId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        modelBuilder.Entity<TaskPriorityEntity>().HasData(
            new TaskPriorityEntity
            {
                Id = lowPriorityId,
                Name = "Low",
                Description = "Low priority task",
                DisplayOrder = 0,
                CreatedAt = DateTime.UtcNow
            },
            new TaskPriorityEntity
            {
                Id = mediumPriorityId,
                Name = "Medium",
                Description = "Medium priority task",
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            },
            new TaskPriorityEntity
            {
                Id = highPriorityId,
                Name = "High",
                Description = "High priority task",
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
