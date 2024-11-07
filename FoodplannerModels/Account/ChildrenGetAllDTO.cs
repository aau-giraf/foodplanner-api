using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

public class ChildrenGetAllDTO
{
    public int ChildId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    public required string ClassName { get; set; }
    public int ClassId { get; set; }
}