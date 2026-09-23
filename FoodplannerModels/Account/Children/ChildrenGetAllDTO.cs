using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

// Child DTO 
// Used in "Get all children in a class", different from ChildrenDTO because it requires class name, but no parents or error messages
public class ChildrenGetAllDTO
{
    public int ChildId { get; set; }
    
    [Required]
    public required string FirstName { get; set; }
    
    [Required]
    public required string LastName { get; set; }
    
    [Required]
    public required string ClassName { get; set; }
    public int ClassId { get; set; }
}