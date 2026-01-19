using System;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService (AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
       await _context.SaveChangesAsync();

        return user;

    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return _context.Users.AsNoTracking().ToList();
    }

    public async Task<User?> GetByIdAsync(int Id)
    {
        return await _context.Users.FirstOrDefaultAsync(p => p.Id == Id);
    }
}
