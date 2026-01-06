namespace BookMyDoctor.Server.DTOs
{
    public class NotificationCreateDto
    {
        public int? UserId { get; set; }
        public string? Message { get; set; }
    }

    public class NotificationUpdateDto
    {
        public string? Message { get; set; }
    }

    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }
        public int? UserId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}