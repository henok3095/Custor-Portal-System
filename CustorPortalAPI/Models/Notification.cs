
namespace CustorPortalAPI.Models
{


    public partial class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? Message { get; set; } = null!;

        public string? Link { get; set; } = null!;

        public bool Read { get; set; }

        public DateTime Timestamp { get; set; }

        public  User? User { get; set; } = null!;
    }
}
