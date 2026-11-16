using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodplannerModels.Tags;

public class Tag
{
    [ForeignKey ("Ingridients")]
    [Required(ErrorMessage = "Ingredient påkrævet")]
    public int Ingredient_id { get; set; }

    [ForeignKey ("Categories")]
    [Required(ErrorMessage = "Kategori påkrævet")]
    public int Category_id { get; set; }
}