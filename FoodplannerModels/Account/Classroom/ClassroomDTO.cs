using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

// Classroom DTO
public class ClassroomDTO
{
    [Key]
    [Required]
    public int ClassId { get; set; }
    
    [Required]
    public required string ClassName { get; set; }
}