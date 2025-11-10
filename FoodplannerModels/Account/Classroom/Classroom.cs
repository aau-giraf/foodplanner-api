namespace FoodplannerModels.Account;
using System.ComponentModel.DataAnnotations;


public class Classroom {
    [Key]
    public int ClassId { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Klasse navn er for langt")]
    public required string ClassName { get; set; }
}