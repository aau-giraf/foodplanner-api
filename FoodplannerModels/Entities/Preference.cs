namespace FoodplannerModels.Preferences;

public class Preferences
{
    public int Id { get; set; }
    public int Child_id { get; set; }
    public string Rating { get; set; }
    public string Food_name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int? Food_image_id { get; set; }
}