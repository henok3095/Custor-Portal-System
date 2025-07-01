namespace CustorPortalAPI.Models
{
    public class UserProject
    {
   
        public int UserKey { get; set; }
        public int ProjectKey { get; set; }
        public string? Role { get; set; }
        public DateTime Assigned_at { get; set; }

        public User? User { get; set; } 
        public Project? Project { get; set; }
    }
}