namespace FoodplannerModels.Lunchbox;

// DTO for transfer of incomplete ingredient data from the frontend to the backend for creating a new meal.
public class IngredientCreateDTO {
    public required string Name {get; set;}
    public required int User_id {get; set;}
    public int? Image_id {get; set;}
}