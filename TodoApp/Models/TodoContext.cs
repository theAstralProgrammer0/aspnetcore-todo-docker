using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TodoApp.Models
{
  public class TodoContext : DbContext
  {
    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; }
  }
}
