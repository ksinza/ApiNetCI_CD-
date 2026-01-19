using System;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

namespace CursoApis.Tests;

public class TaskServiceTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        return context;
    }

    [Fact]
    public async Task CreateAsync_WithValidTask_ShouldAddTaskToDatabase()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TaskService(context);
        var newTask = new TaskItem
        {
            Title = "Test Task",
            IsCompleted = false,
            UserId = 1
        };

        // Act
        var result = await service.CreateAsync(newTask);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.False(result.IsCompleted);
        Assert.Equal(1, result.UserId);
        
        // Verify it was saved to the database
        var taskInDb = await context.Tasks.FirstOrDefaultAsync(t => t.Title == "Test Task");
        Assert.NotNull(taskInDb);
    }

    [Fact]
    public async Task CreateAsync_WithMultipleTasks_ShouldAddAllTasksToDatabase()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TaskService(context);
        var task1 = new TaskItem { Title = "Task 1", IsCompleted = false, UserId = 1 };
        var task2 = new TaskItem { Title = "Task 2", IsCompleted = true, UserId = 2 };

        // Act
        await service.CreateAsync(task1);
        await service.CreateAsync(task2);

        // Assert
        var allTasks = await service.GetAllAsync();
        Assert.Equal(2, allTasks.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithNoTasks_ShouldReturnEmptyList()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TaskService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleTasks_ShouldReturnAllTasks()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TaskService(context);
        
        var task1 = new TaskItem { Title = "Task 1", IsCompleted = false, UserId = 1 };
        var task2 = new TaskItem { Title = "Task 2", IsCompleted = true, UserId = 2 };
        var task3 = new TaskItem { Title = "Task 3", IsCompleted = false, UserId = 1 };

        await service.CreateAsync(task1);
        await service.CreateAsync(task2);
        await service.CreateAsync(task3);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, t => t.Title == "Task 1");
        Assert.Contains(result, t => t.Title == "Task 2");
        Assert.Contains(result, t => t.Title == "Task 3");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTask()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TaskService(context);
        var newTask = new TaskItem { Title = "Find Me", IsCompleted = false, UserId = 1 };
        var createdTask = await service.CreateAsync(newTask);

        // Act
        var result = await service.GetByIdAsync(createdTask.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdTask.Id, result.Id);
        Assert.Equal("Find Me", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TaskService(context);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectTaskWhenMultipleExist()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TaskService(context);
        
        var task1 = new TaskItem { Title = "Task 1", IsCompleted = false, UserId = 1 };
        var task2 = new TaskItem { Title = "Task 2", IsCompleted = true, UserId = 2 };
        var task3 = new TaskItem { Title = "Task 3", IsCompleted = false, UserId = 3 };

        var createdTask1 = await service.CreateAsync(task1);
        var createdTask2 = await service.CreateAsync(task2);
        var createdTask3 = await service.CreateAsync(task3);

        // Act
        var result = await service.GetByIdAsync(createdTask2.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdTask2.Id, result.Id);
        Assert.Equal("Task 2", result.Title);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetIdOnCreatedTask()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new TaskService(context);
        var newTask = new TaskItem { Title = "Task with ID", IsCompleted = false, UserId = 1 };

        // Act
        var result = await service.CreateAsync(newTask);

        // Assert
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateAsync_MultipleCallsWithDifferentDatabase_ShouldIsolateData()
    {
        // Arrange
        var context1 = CreateInMemoryContext();
        var context2 = CreateInMemoryContext();
        var service1 = new TaskService(context1);
        var service2 = new TaskService(context2);
        
        var task1 = new TaskItem { Title = "Task in DB1", IsCompleted = false, UserId = 1 };
        var task2 = new TaskItem { Title = "Task in DB2", IsCompleted = false, UserId = 1 };

        // Act
        await service1.CreateAsync(task1);
        await service2.CreateAsync(task2);
        
        var result1 = await service1.GetAllAsync();
        var result2 = await service2.GetAllAsync();

        // Assert
        Assert.Single(result1);
        Assert.Single(result2);
        Assert.Equal("Task in DB1", result1.First().Title);
        Assert.Equal("Task in DB2", result2.First().Title);
    }
}
