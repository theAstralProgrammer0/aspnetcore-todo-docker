using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
  public class TodoItem
  {
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Name is Required")]
    [StringLength(200, ErrorMessage = "Name must be less than 200 characters")]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    public bool IsCompleted { get; set; } = false;

    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

    public DateTime? CompletedAt { get; set; }

    public DateTime DueDate { get; set; } = DateTime.MaxValue;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }

  public enum PriorityLevel
  {
    Low,
    Medium,
    High
  }
}
