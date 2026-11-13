namespace FoodplannerModels.Codes;
using System.ComponentModel.DataAnnotations;
public class OneTimePassword
{
    [Key]
    public int CodeId {  get; set; }
    required public int GeneratedBy {  get; set; }
    required public string Code { get; set; }

    required public DateTime CreatedOn { get; set; }
    required public DateTime ExpiresOn { get; set; }
    required public bool Used {  get; set; } 
    public int? UsedByUser { get; set; }
    public int? ChildUser { get; set; }
}