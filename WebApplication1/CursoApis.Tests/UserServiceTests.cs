using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;
using Xunit;

namespace CursoApis.Tests;

public class UserServiceTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_WithValidUser_ShouldAddUserToDatabase()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);
        var user = new User { Name = "Test User", Email = "test@example.com", Role = "User" };

        // Act
        var created = await service.CreateAsync(user);

        // Assert
        Assert.NotNull(created);
        Assert.Equal("Test User", created.Name);
        Assert.Equal("test@example.com", created.Email);
        Assert.Equal("User", created.Role);

        var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(userInDb);
    }

    [Fact]
    public async Task GetAllAsync_WithNoUsers_ShouldReturnEmpty()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleUsers_ShouldReturnAll()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);

        var u1 = new User { Name = "A", Email = "a@example.com", Role = "Admin" };
        var u2 = new User { Name = "B", Email = "b@example.com", Role = "User" };

        await service.CreateAsync(u1);
        await service.CreateAsync(u2);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.Email == "a@example.com");
        Assert.Contains(result, u => u.Email == "b@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);
        var user = new User { Name = "Find", Email = "find@example.com", Role = "User" };
        var created = await service.CreateAsync(user);

        // Act
        var result = await service.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("find@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);

        // Act
        var result = await service.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetIdOnCreatedUser()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var service = new UserService(context);
        var user = new User { Name = "WithId", Email = "id@example.com", Role = "User" };

        // Act
        var created = await service.CreateAsync(user);

        // Assert
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task CreateAsync_IsolatedInMemoryDatabases_ShouldNotShareData()
    {
        // Arrange
        var ctx1 = CreateInMemoryContext();
        var ctx2 = CreateInMemoryContext();
        var svc1 = new UserService(ctx1);
        var svc2 = new UserService(ctx2);

        var u1 = new User { Name = "U1", Email = "u1@example.com", Role = "User" };
        var u2 = new User { Name = "U2", Email = "u2@example.com", Role = "User" };

        // Act
        await svc1.CreateAsync(u1);
        await svc2.CreateAsync(u2);

        var r1 = await svc1.GetAllAsync();
        var r2 = await svc2.GetAllAsync();

        // Assert
        Assert.Single(r1);
        Assert.Single(r2);
        Assert.Equal("u1@example.com", r1.First().Email);
        Assert.Equal("u2@example.com", r2.First().Email);
    }
}
