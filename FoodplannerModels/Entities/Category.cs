using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Tags;
public class Category
{
    [Key]
    [Required]
    public int Id { get; set; }
    public required string Name { get; set;}

    // Indicate if category is a main category (vegetables, meat, dairy, etc.) 
    // or a sub-tag/sub-category (chopped, sliced, etc.)
    [Required]
    public bool Main { get; set; }
}