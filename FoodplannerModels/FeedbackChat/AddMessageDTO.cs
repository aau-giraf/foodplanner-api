using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.FeedbackChat;

public class AddMessageDTO
{
    [Required]
    public required int ChatThreadId { get; set; }
    
    [Required]
    public required string Content { get; set; }
}