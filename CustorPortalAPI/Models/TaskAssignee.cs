namespace CustorPortalAPI.Models
{
    public class TaskAssignee
    {
        public int Taskkey { get; set; }
        public int UserKey { get; set; }

        public Task? Task { get; set; }
        public User? User { get; set; }
    }
}