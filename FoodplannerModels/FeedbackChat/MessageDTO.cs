public class MessageDTO
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public int SentByUserId { get; set; }
    public int ChatThreadId { get; set; }
}