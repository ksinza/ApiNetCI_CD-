using System;
using WebApplication1.Models;

namespace WebApplication1.Services;

public interface ITaskService
{

    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> CreateAsync(TaskItem task);
}
