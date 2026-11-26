namespace FoodplannerModels.Account;

// DTO for a child's Preference 
public class PreferenceDTO
{
    public required int Id { get; set; }

    public required int Child_id { get; set; }

    public required int Food_image_id { get; set; }

    public required DateOnly Date { get; set; }

    public required string Name { get; set; }

    public required int Rating { get; set; }

}