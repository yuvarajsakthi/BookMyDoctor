namespace BookMyDoctor.Server.DTOs
{
    public class MessageCreateDto
    {
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string? MessageText { get; set; }
    }

    public class MessageUpdateDto
    {
        public string? MessageText { get; set; }
    }

    public class MessageResponseDto
    {
        public int MessageId { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string? MessageText { get; set; }
        public DateTime SentAt { get; set; }
    }
}