namespace FoodplannerModels.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Children {
    [Key]
    public int ChildId { get; set; }
    
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    [ForeignKey("Classroom")]
    public int classId { get; set; }
}

