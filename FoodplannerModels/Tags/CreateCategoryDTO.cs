using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Tags;
public class CreateCategoryDTO
{

    public required string Name { get; set;}

    // Indicate if category is a main category (vegetables, meat, dairy, etc.) 
    // or a sub-tag/sub-category (chopped, sliced, etc.)
    public bool Main { get; set; }
}