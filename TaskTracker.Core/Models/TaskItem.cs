namespace TaskTracker.Core.Models;

public class TaskItem
{
    public int Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public TaskItem(int id, string title)
    {
        Id = id;
        Title = title;
    }
    
    public TaskItem() { }
    
    public string Status => IsComplete ? "✓" : "○";
    
    public override string ToString() => $"[{Status}] {Id}: {Title}";
}
