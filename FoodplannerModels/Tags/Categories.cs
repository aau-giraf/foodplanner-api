using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Tags;
public class Categories
{
    [Key]
    public int Id {get; set; }
    public string Name {get; set;}
    public bool Main {get; set;}
}