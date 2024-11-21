public class MessageDTO
{
    public int MessageId { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public int ChatThreadId { get; set; }
}