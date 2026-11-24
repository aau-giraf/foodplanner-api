using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ChatThread
{
    [Key]
    public required int ChatThreadId { get; set; }

    [ForeignKey("ChildId")]
    public required int ChildId { get; set; }
}
