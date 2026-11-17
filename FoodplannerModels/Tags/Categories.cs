using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Tags;
public class Categories
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set;}

    // Indicate if category is a main category (vegetables, meat, dairy, etc.) 
    // or a sub-tag/sub-category (chopped, sliced, etc.)
    [Required]
    public bool Main { get; set; }
}