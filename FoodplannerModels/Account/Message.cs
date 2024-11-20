using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Message
{
    [Key]
    public int MessageId { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public DateTime SentAt { get; set; }

    [ForeignKey("SentByUserId")]
    public int SentByUserId { get; set; }
    
    [ForeignKey("RecievedByUserId")]
    public int RecievedByUserId { get; set; }

    [ForeignKey("ChatThreadId")]
    public int ChatThreadId { get; set; }
}