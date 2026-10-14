// Adds the Meal class to the FoodplannerModels.Lunchbox namespace
namespace FoodplannerModels.Lunchbox;

// DTO for transfer of backend data to the frontend for a meal.
public class MealDTO {
    public required int Id {get; init;}
    public required string Name {get; set;}
    public required string Date {get; set;}
    public required bool Template {get; set;}
    public int? Food_image_id {get; set;}
    public required int User_id {get; init;}
}