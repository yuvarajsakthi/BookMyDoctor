using System.ComponentModel.DataAnnotations;

namespace BookMyDoctor.Server.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int? UserId { get; set; }
        public string? Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        
        public User? User { get; set; }
    }
}