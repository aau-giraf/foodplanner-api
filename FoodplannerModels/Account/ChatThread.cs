using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ChatThread
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("MessageId")]
    public int MessageId { get; set; }
    
}
