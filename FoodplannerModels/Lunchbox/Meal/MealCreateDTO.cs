namespace FoodplannerModels.Lunchbox;

// DTO for transfer of incomplete meal data from the frontend to the backend for creating a new meal.
public class MealCreateDTO
{
    public required string Name {get; set;}
    public required string Date {get; set;}
    public required bool Template {get; set;}
    public int? Image_id {get; set;}
}