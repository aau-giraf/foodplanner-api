namespace FoodplannerModels.Codes;
using System.ComponentModel.DataAnnotations;
public class OneTimePasswordDTO
{
    [Required]
    required public int CodeId {  get; set; }
    [Required]
    required public int GeneratedBy {  get; set; }
    [Required]
    required public bool Used {  get; set; } 
    public int? UsedByUser { get; set; }
    public int? ChildUser { get; set; }
}
