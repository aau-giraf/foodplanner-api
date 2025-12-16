using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ChatThreadDTO
{
    [Key]
    [Required]
    public required int Id { get; set; }
    
    [ForeignKey("ChildId")]
    [Required]
    public required int ChildId { get; set; }
}