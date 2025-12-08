namespace FoodplannerModels.Codes;
using System.ComponentModel.DataAnnotations;
public class OneTimePasswordDTO
{
    required public int CodeId {  get; set; }
    required public int GeneratedBy {  get; set; }
    required public bool Used {  get; set; } 
    public int? UsedByUser { get; set; }
    public int? ChildUser { get; set; }
}
