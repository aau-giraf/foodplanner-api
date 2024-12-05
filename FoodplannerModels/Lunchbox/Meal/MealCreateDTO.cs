namespace FoodplannerModels.Lunchbox;

public class MealCreateDTO
{

    public int? Food_image_id { get; set; }
    // Title of the meal.
    public required string Name { get; set; }
    // Date accosiated with the meal
    public required string Date { get; set; }
}