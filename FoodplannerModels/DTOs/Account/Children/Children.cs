namespace FoodplannerModels.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


// Child entity 
// Used in controllers, repositories, and services 
public class Children {
    [Key, ForeignKey("User")]
    public required int ChildId { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    [ForeignKey("Classroom")]
    public int? ClassId { get; set; }
}

