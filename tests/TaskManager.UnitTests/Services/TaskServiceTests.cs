using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskManager.UnitTests.Builders;
using TaskManager.UnitTests.Helpers;
using Xunit;

namespace TaskManager.UnitTests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<TaskService>> _mockLogger;
    private readonly TaskService _sut; // System Under Test
    private readonly string _testUserId;

    public TaskServiceTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<TaskService>>();
        _sut = new TaskService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
        _testUserId = FakerHelper.GenerateUserId();
    }

    #region GetAllTasksAsync Tests

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnUserTasks_WhenTasksExist()
    {
        // Arrange
        var userTasks = new List<TaskItem>
        {
            TaskItemBuilder.ForUser(_testUserId).WithTitle("User Task 1").Build(),
            TaskItemBuilder.ForUser(_testUserId).WithTitle("User Task 2").Build()
        };

        var otherUserTasks = new List<TaskItem>
        {
            TaskItemBuilder.ForUser(FakerHelper.GenerateUserId()).WithTitle("Other User Task").Build()
        };

        var allTasks = userTasks.Concat(otherUserTasks).ToList();
        var expectedDtos = FakerHelper.TaskDtoFaker.Generate(2);

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(allTasks);

        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(It.IsAny<IEnumerable<TaskItem>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _sut.GetAllTasksAsync(_testUserId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<TaskDto>>(It.Is<IEnumerable<TaskItem>>(
            tasks => tasks.All(t => t.UserId == _testUserId))), Times.Once);
    }

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnEmptyList_WhenNoTasksExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TaskItem>());

        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(It.IsAny<IEnumerable<TaskItem>>()))
            .Returns(new List<TaskDto>());

        // Act
        var result = await _sut.GetAllTasksAsync(_testUserId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllTasksAsync_ShouldFilterByUserId_WhenMultipleUsersHaveTasks()
    {
        // Arrange
        var user1Id = FakerHelper.GenerateUserId();
        var user2Id = FakerHelper.GenerateUserId();

        var allTasks = new List<TaskItem>
        {
            TaskItemBuilder.ForUser(user1Id).Build(),
            TaskItemBuilder.ForUser(user1Id).Build(),
            TaskItemBuilder.ForUser(user2Id).Build(),
            TaskItemBuilder.ForUser(user2Id).Build(),
            TaskItemBuilder.ForUser(user2Id).Build()
        };

        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(allTasks);

        _mockMapper.Setup(m => m.Map<IEnumerable<TaskDto>>(It.IsAny<IEnumerable<TaskItem>>()))
            .Returns((IEnumerable<TaskItem> tasks) => tasks.Select(t => new TaskDto { Id = t.Id }));

        // Act
        var result = await _sut.GetAllTasksAsync(user1Id);

        // Assert
        result.Should().HaveCount(2);
        _mockMapper.Verify(m => m.Map<IEnumerable<TaskDto>>(It.Is<IEnumerable<TaskItem>>(
            tasks => tasks.Count() == 2 && tasks.All(t => t.UserId == user1Id))), Times.Once);
    }

    #endregion

    #region GetTaskByIdAsync Tests

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExistsAndBelongsToUser()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = TaskItemBuilder.ForUser(_testUserId)
            .WithId(taskId)
            .WithTitle("Test Task")
            .Build();

        var expectedDto = FakerHelper.TaskDtoFaker.Generate();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        _mockMapper.Setup(m => m.Map<TaskDto>(task))
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetTaskByIdAsync(taskId, _testUserId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedDto);
        _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mockMapper.Verify(m => m.Map<TaskDto>(task), Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.GetTaskByIdAsync(taskId, _testUserId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mockMapper.Verify(m => m.Map<TaskDto>(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnNull_WhenTaskBelongsToDifferentUser()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var differentUserId = FakerHelper.GenerateUserId();
        var task = TaskItemBuilder.ForUser(differentUserId)
            .WithId(taskId)
            .Build();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _sut.GetTaskByIdAsync(taskId, _testUserId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mockMapper.Verify(m => m.Map<TaskDto>(It.IsAny<TaskItem>()), Times.Never);
    }

    #endregion

    #region CreateTaskAsync Tests

    [Fact]
    public async Task CreateTaskAsync_ShouldCreateTask_WithCorrectProperties()
    {
        // Arrange
        var createDto = CreateTaskDtoBuilder.Default()
            .WithTitle("New Task")
            .WithDescription("Task Description")
            .WithHighPriority()
            .Build();

        var mappedTask = TaskItemBuilder.Default()
            .WithTitle(createDto.Title)
            .WithDescription(createDto.Description)
            .WithPriority(createDto.Priority)
            .Build();

        var createdTask = TaskItemBuilder.ForUser(_testUserId)
            .WithId(Guid.NewGuid())
            .WithTitle(createDto.Title)
            .WithStatus(TaskStatus.Todo)
            .WithIsCompleted(false)
            .Build();

        var expectedDto = FakerHelper.TaskDtoFaker.Generate();

        _mockMapper.Setup(m => m.Map<TaskItem>(createDto))
            .Returns(mappedTask);

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(createdTask);

        _mockMapper.Setup(m => m.Map<TaskDto>(createdTask))
            .Returns(expectedDto);

        // Act
        var result = await _sut.CreateTaskAsync(createDto, _testUserId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedDto);

        _mockRepository.Verify(r => r.CreateAsync(It.Is<TaskItem>(t =>
            t.UserId == _testUserId &&
            t.StatusId == TaskLookupIds.TodoStatusId &&
            t.IsCompleted == false &&
            t.CreatedAt != default
        )), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldSetDefaultStatus_ToTodo()
    {
        // Arrange
        var createDto = CreateTaskDtoBuilder.Default().Build();
        var mappedTask = TaskItemBuilder.Default().Build();
        var createdTask = TaskItemBuilder.Default().Build();
        var expectedDto = FakerHelper.TaskDtoFaker.Generate();

        _mockMapper.Setup(m => m.Map<TaskItem>(createDto)).Returns(mappedTask);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<TaskItem>())).ReturnsAsync(createdTask);
        _mockMapper.Setup(m => m.Map<TaskDto>(createdTask)).Returns(expectedDto);

        // Act
        await _sut.CreateTaskAsync(createDto, _testUserId);

        // Assert
        _mockRepository.Verify(r => r.CreateAsync(It.Is<TaskItem>(t =>
            t.StatusId == TaskLookupIds.TodoStatusId
        )), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldSetIsCompleted_ToFalse()
    {
        // Arrange
        var createDto = CreateTaskDtoBuilder.Default().Build();
        var mappedTask = TaskItemBuilder.Default().Build();
        var createdTask = TaskItemBuilder.Default().Build();
        var expectedDto = FakerHelper.TaskDtoFaker.Generate();

        _mockMapper.Setup(m => m.Map<TaskItem>(createDto)).Returns(mappedTask);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<TaskItem>())).ReturnsAsync(createdTask);
        _mockMapper.Setup(m => m.Map<TaskDto>(createdTask)).Returns(expectedDto);

        // Act
        await _sut.CreateTaskAsync(createDto, _testUserId);

        // Assert
        _mockRepository.Verify(r => r.CreateAsync(It.Is<TaskItem>(t =>
            t.IsCompleted == false
        )), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldAssignNewGuid()
    {
        // Arrange
        var createDto = CreateTaskDtoBuilder.Default().Build();
        var mappedTask = TaskItemBuilder.Default().Build();
        var createdTask = TaskItemBuilder.Default().Build();
        var expectedDto = FakerHelper.TaskDtoFaker.Generate();

        _mockMapper.Setup(m => m.Map<TaskItem>(createDto)).Returns(mappedTask);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<TaskItem>())).ReturnsAsync(createdTask);
        _mockMapper.Setup(m => m.Map<TaskDto>(createdTask)).Returns(expectedDto);

        // Act
        await _sut.CreateTaskAsync(createDto, _testUserId);

        // Assert
        _mockRepository.Verify(r => r.CreateAsync(It.Is<TaskItem>(t =>
            t.Id != Guid.Empty
        )), Times.Once);
    }

    #endregion

    #region UpdateTaskAsync Tests

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTask_WhenTaskExistsAndBelongsToUser()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = TaskItemBuilder.ForUser(_testUserId)
            .WithId(taskId)
            .WithTitle("Old Title")
            .Build();

        var updateDto = UpdateTaskDtoBuilder.Default()
            .WithTitle("Updated Title")
            .WithDescription("Updated Description")
            .Build();

        var updatedTask = TaskItemBuilder.ForUser(_testUserId)
            .WithId(taskId)
            .WithTitle("Updated Title")
            .Build();

        var expectedDto = FakerHelper.TaskDtoFaker.Generate();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(existingTask);

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(updatedTask);

        _mockMapper.Setup(m => m.Map<TaskDto>(updatedTask))
            .Returns(expectedDto);

        // Act
        var result = await _sut.UpdateTaskAsync(taskId, updateDto, _testUserId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedDto);

        _mockMapper.Verify(m => m.Map(updateDto, existingTask), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t =>
            t.UpdatedAt != null
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldThrowKeyNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var updateDto = UpdateTaskDtoBuilder.Default().Build();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var act = async () => await _sut.UpdateTaskAsync(taskId, updateDto, _testUserId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Task with ID {taskId} not found");

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldThrowUnauthorizedAccessException_WhenTaskBelongsToDifferentUser()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var differentUserId = FakerHelper.GenerateUserId();
        var existingTask = TaskItemBuilder.ForUser(differentUserId)
            .WithId(taskId)
            .Build();

        var updateDto = UpdateTaskDtoBuilder.Default().Build();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(existingTask);

        // Act
        var act = async () => await _sut.UpdateTaskAsync(taskId, updateDto, _testUserId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not authorized to update this task");

        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldSetUpdatedAt_ToCurrentDateTime()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var beforeUpdate = DateTime.UtcNow;
        var existingTask = TaskItemBuilder.ForUser(_testUserId).WithId(taskId).Build();
        var updateDto = UpdateTaskDtoBuilder.Default().Build();
        var updatedTask = TaskItemBuilder.Default().Build();
        var expectedDto = FakerHelper.TaskDtoFaker.Generate();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).ReturnsAsync(updatedTask);
        _mockMapper.Setup(m => m.Map<TaskDto>(updatedTask)).Returns(expectedDto);

        // Act
        await _sut.UpdateTaskAsync(taskId, updateDto, _testUserId);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t =>
            t.UpdatedAt >= beforeUpdate && t.UpdatedAt <= afterUpdate
        )), Times.Once);
    }

    #endregion

    #region DeleteTaskAsync Tests

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnTrue_WhenTaskDeletedSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = TaskItemBuilder.ForUser(_testUserId)
            .WithId(taskId)
            .Build();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(existingTask);

        _mockRepository.Setup(r => r.DeleteAsync(taskId))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteTaskAsync(taskId, _testUserId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.DeleteTaskAsync(taskId, _testUserId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldThrowUnauthorizedAccessException_WhenTaskBelongsToDifferentUser()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var differentUserId = FakerHelper.GenerateUserId();
        var existingTask = TaskItemBuilder.ForUser(differentUserId)
            .WithId(taskId)
            .Build();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(existingTask);

        // Act
        var act = async () => await _sut.DeleteTaskAsync(taskId, _testUserId);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not authorized to delete this task");

        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnFalse_WhenRepositoryReturnsFalse()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = TaskItemBuilder.ForUser(_testUserId)
            .WithId(taskId)
            .Build();

        _mockRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(existingTask);

        _mockRepository.Setup(r => r.DeleteAsync(taskId))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.DeleteTaskAsync(taskId, _testUserId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteAsync(taskId), Times.Once);
    }

    #endregion
}
