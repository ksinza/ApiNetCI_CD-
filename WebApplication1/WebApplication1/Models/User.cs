using System;
using System.Security.Cryptography.X509Certificates;

namespace WebApplication1.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Role { get; set; } = default!;
    public ICollection<TaskItem> Tasks = new List<TaskItem>();

}
