# TaskManager Unit Tests

This project contains unit tests for the TaskManager application, following best practices with xUnit, Bogus Faker, and the Builder pattern.

## Technologies Used

- **xUnit**: Testing framework
- **Moq**: Mocking library for creating test doubles
- **Bogus**: Library for generating realistic fake data
- **FluentAssertions**: Fluent API for writing expressive and readable assertions

## Project Structure

```
TaskManager.UnitTests/
├── Builders/              # Builder pattern implementations
│   ├── TaskItemBuilder.cs
│   ├── CreateTaskDtoBuilder.cs
│   └── UpdateTaskDtoBuilder.cs
├── Helpers/               # Test helpers and faker configurations
│   └── FakerHelper.cs
└── Services/              # Service layer unit tests
    └── TaskServiceTests.cs
```

## Using the Builders

The builders use the **Builder Pattern** combined with **Bogus Faker** to create test data. Each builder starts with faker-generated data and allows you to customize specific properties.

### TaskItemBuilder

Create test TaskItem entities with custom properties:

```csharp
// Create a basic task with random data
var task = TaskItemBuilder.Default().Build();

// Create a task for a specific user
var userTask = TaskItemBuilder.ForUser(userId)
    .WithTitle("My Custom Task")
    .WithHighPriority()
    .AsTodoTask()
    .Build();

// Create a completed task
var completedTask = TaskItemBuilder.Default()
    .WithUserId(userId)
    .AsCompletedTask()
    .WithDueDate(DateTime.Now.AddDays(7))
    .Build();
```

### CreateTaskDtoBuilder

Create test CreateTaskDto objects:

```csharp
// Basic creation DTO
var createDto = CreateTaskDtoBuilder.Default().Build();

// Customized creation DTO
var customDto = CreateTaskDtoBuilder.Default()
    .WithTitle("New Feature")
    .WithDescription("Implement new feature")
    .WithHighPriority()
    .WithDueDate(DateTime.Now.AddDays(14))
    .Build();
```

### UpdateTaskDtoBuilder

Create test UpdateTaskDto objects:

```csharp
// Basic update DTO
var updateDto = UpdateTaskDtoBuilder.Default().Build();

// Mark task as completed
var completeDto = UpdateTaskDtoBuilder.Default()
    .AsCompletedTask()
    .Build();

// Update to in-progress
var progressDto = UpdateTaskDtoBuilder.Default()
    .AsInProgressTask()
    .WithHighPriority()
    .Build();
```

## Using the Faker Helper

The `FakerHelper` provides pre-configured Bogus fakers for all domain entities and DTOs:

```csharp
// Generate a single task item
var task = FakerHelper.GenerateTaskItem(userId);

// Generate multiple task items
var tasks = FakerHelper.GenerateTaskItems(10, userId);

// Use the faker directly
var taskDto = FakerHelper.TaskDtoFaker.Generate();
var createDto = FakerHelper.CreateTaskDtoFaker.Generate();

// Generate a user ID
var userId = FakerHelper.GenerateUserId();
```

## Writing Unit Tests

### Test Structure

All tests follow the **Arrange-Act-Assert (AAA)** pattern:

```csharp
[Fact]
public async Task MethodName_ShouldExpectedBehavior_WhenCondition()
{
    // Arrange - Set up test data and mocks
    var taskId = Guid.NewGuid();
    var task = TaskItemBuilder.ForUser(_testUserId)
        .WithId(taskId)
        .Build();
    
    _mockRepository.Setup(r => r.GetByIdAsync(taskId))
        .ReturnsAsync(task);

    // Act - Execute the method being tested
    var result = await _sut.GetTaskByIdAsync(taskId, _testUserId);

    // Assert - Verify the expected outcome
    result.Should().NotBeNull();
    _mockRepository.Verify(r => r.GetByIdAsync(taskId), Times.Once);
}
```

### Test Naming Convention

Tests follow the pattern: `MethodName_ShouldExpectedBehavior_WhenCondition`

Examples:
- `GetTaskByIdAsync_ShouldReturnTask_WhenTaskExistsAndBelongsToUser`
- `CreateTaskAsync_ShouldSetDefaultStatus_ToTodo`
- `DeleteTaskAsync_ShouldReturnFalse_WhenTaskDoesNotExist`

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run tests with detailed output
```bash
dotnet test --logger "console;verbosity=normal"
```

### Run tests for specific project
```bash
dotnet test tests/TaskManager.UnitTests/TaskManager.UnitTests.csproj
```

### Run tests with coverage (requires coverlet)
```bash
dotnet test /p:CollectCoverage=true
```

## Test Coverage

The current test suite covers:

- ✅ TaskService.GetAllTasksAsync
  - Returns user tasks when tasks exist
  - Returns empty list when no tasks exist
  - Filters by user ID correctly

- ✅ TaskService.GetTaskByIdAsync
  - Returns task when it exists and belongs to user
  - Returns null when task doesn't exist
  - Returns null when task belongs to different user

- ✅ TaskService.CreateTaskAsync
  - Creates task with correct properties
  - Sets default status to Todo
  - Sets IsCompleted to false
  - Assigns new GUID

- ✅ TaskService.UpdateTaskAsync
  - Updates task when it exists and belongs to user
  - Throws KeyNotFoundException when task doesn't exist
  - Throws UnauthorizedAccessException when task belongs to different user
  - Sets UpdatedAt timestamp

- ✅ TaskService.DeleteTaskAsync
  - Returns true when task deleted successfully
  - Returns false when task doesn't exist
  - Throws UnauthorizedAccessException when task belongs to different user
  - Returns false when repository returns false

## Best Practices

1. **Use Builders for Complex Objects**: The builder pattern makes test data creation readable and maintainable
2. **Use Faker for Realistic Data**: Bogus generates realistic test data, catching issues that simple hardcoded values might miss
3. **Mock External Dependencies**: Use Moq to isolate the system under test
4. **Use FluentAssertions**: Write expressive, readable assertions
5. **Follow AAA Pattern**: Keep tests organized with clear Arrange-Act-Assert sections
6. **Test One Thing**: Each test should verify a single behavior
7. **Name Tests Descriptively**: Test names should clearly describe what is being tested

## Adding New Tests

When adding new tests:

1. Create or extend builders in the `Builders/` directory
2. Add faker configurations to `FakerHelper.cs` if needed
3. Follow the existing naming conventions
4. Use the builder pattern for test data
5. Include both positive and negative test cases
6. Verify all mocks and assertions

## Dependencies

All test dependencies are managed via NuGet packages defined in `TaskManager.UnitTests.csproj`:

- xunit (>= 3.x)
- Moq (>= 4.x)
- Bogus (>= 35.x)
- FluentAssertions (>= 8.x)
- xunit.runner.visualstudio
- Microsoft.NET.Test.Sdk
