using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

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