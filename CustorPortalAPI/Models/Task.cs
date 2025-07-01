using System.ComponentModel.DataAnnotations;

namespace CustorPortalAPI.Models
{
    public class Task
    {
        public int TaskKey { get; set; }
        public int ProjectKey { get; set; }
        [Required,MaxLength(100)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        [RegularExpression("^(To Do|In Progress|In Review|Done)$")]
        public string? Status { get; set; }
        [Required]
        [RegularExpression("^(Low|Medium|High)$")]
        public string? Priority { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        public DateTime? Deadline { get; set; }
        public int CreatorKey { get; set; }

        public Project? Project { get; set; }
        public User? Creator { get; set; }
        public ICollection<TaskAssignee>? TaskAssignees { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}