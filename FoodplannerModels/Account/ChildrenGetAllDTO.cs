using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

public class ChildrenGetAllDTO
{
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string ClassName { get; set; }
}