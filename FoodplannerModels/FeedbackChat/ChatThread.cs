using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ChatThread
{
    [Key]
    public int ChatThreadId { get; set; }

    [ForeignKey("ChildId")]
    public int ChildId { get; set; }
    
    
    
}
