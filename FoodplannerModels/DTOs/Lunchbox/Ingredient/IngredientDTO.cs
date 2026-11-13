namespace FoodplannerModels.Lunchbox;

// DTO for representing a row in the database table ingredients.
public class IngredientDTO {
    public required int Id {get; init;}
    public required string Name {get; set;}
    public required int User_id {get; set;}
    public int? Image_id {get; set;}
}