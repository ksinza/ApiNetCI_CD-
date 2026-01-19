using System;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
       _context.Tasks.Add(task);
       await _context.SaveChangesAsync();
       return task;
    }

    public  async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FirstOrDefaultAsync(u=> u.Id == id);
    }
}
