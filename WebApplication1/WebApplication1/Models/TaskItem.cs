using System;

namespace WebApplication1.Models;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public bool IsCompleted { get; set; }

    public int UserId { get; set;}

    public User? User { get; set; }

}
