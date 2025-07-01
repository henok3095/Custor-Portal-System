public class Task
{
    public int TaskKey { get; set; }
    public int ProjectKey { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public int CreatorKey { get; set; }
    public DateTime CreatedAt { get; set; }

    // Constructor matching the arguments used in the code  
    public Task(int projectKey, string title, string? description, string status, string priority, DateTime? deadline, int creatorKey, DateTime createdAt)
    {
        ProjectKey = projectKey;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        Deadline = deadline;
        CreatorKey = creatorKey;
        CreatedAt = createdAt;
    }
}
