using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Message
{
    [Key]
    public int MessageId { get; set; }

    [Required(ErrorMessage = "Shanice, your mouth is moving like a waterfall, but nothing is coming out")]
    [StringLength(500, ErrorMessage = "Shanice, your mouth is ehhh moving alot like a rat, yappa yappa yappa yappa, shut it please!")]
    public string Content { get; set; }

    [Required]
    public DateTime SentAt { get; set; }

    [ForeignKey("SentByUserId")]
    public int SentByUserId { get; set; }

    [ForeignKey("ChatThreadId")]
    public int ChatThreadId { get; set; }
    
    public bool Archived { get; set; }
}