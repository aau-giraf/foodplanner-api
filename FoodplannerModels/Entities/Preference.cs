namespace FoodplannerModels.Preferences;

public class Preference
{
    public int ChildId { get; set; }
    public int Rating { get; set; }
    public string FoodName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int? ImageId { get; set; }
}