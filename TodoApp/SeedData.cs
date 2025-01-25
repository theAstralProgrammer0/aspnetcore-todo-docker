using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Models;

public static class SeedData
{
  public static void Initialize(IServiceProvider serviceProvider)
  {
    using var context = new TodoContext(
        serviceProvider.GetRequiredService<DbContextOptions<TodoContext>>());
    
    if (context.TodoItems.Any()) return; // Db has been seeded

    context.TodoItems.AddRange(
        new TodoItem
        {
            Name = "Buy groceries",
            Description = "Milk, Eggs, Bread, Cheese",
            IsCompleted = false,
            Priority = PriorityLevel.High,
            DueDate = DateTime.UtcNow.AddDays(2)
        },
        new TodoItem
        {
            Name = "Call Mom",
            Description = "Wish her a happy birthday",
            IsCompleted = false,
            Priority = PriorityLevel.Medium,
            DueDate = DateTime.UtcNow.AddDays(1)
        }
    );

    context.SaveChanges();
  }
}
