using System.Text.Json;

namespace FoodplannerModels.Account;

public class UserCredsDTO
{
    public required string JWT { get; set; }
    public required string Role { get; set; }
    public required string Status { get; set; }
}