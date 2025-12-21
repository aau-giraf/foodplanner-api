namespace FoodplannerModels.Account;
using System.ComponentModel.DataAnnotations;


public class Classroom {
    [Key]
    public int ClassId { get; set; }
    public required string ClassName { get; set; }
}