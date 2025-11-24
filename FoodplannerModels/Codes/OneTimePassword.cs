namespace FoodplannerModels.Codes;
using System.ComponentModel.DataAnnotations;
public class OneTimePassword
{
    [Key]
    public int CodeId {  get; set; }

    public int GeneratedBy {  get; set; }

    [StringLength(6)]
    public string Code { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime ExpiresOn { get; set; }
    public bool Used {  get; set; } 
    public int? UsedByUser { get; set; }

    public OneTimePassword()
    {
        Code = "";
    }
}
