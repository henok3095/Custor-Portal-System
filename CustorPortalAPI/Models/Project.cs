namespace CustorPortalAPI.Models
{
    public class Project
    {
        public int ProjectKey { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        public int creatorKey { get; set; }
        public User? Creator { get; set; }
        public ICollection<Task>? Tasks { get; set; }
        public ICollection<File>? Files { get; set; }
        public ICollection<UserProject>? UserProjects { get; set; } // Fix: Added missing property
      
    }
}
