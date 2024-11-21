using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ChatThread
{
    [Key]
    public int ChatThreadId { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }
    
    
    
}
