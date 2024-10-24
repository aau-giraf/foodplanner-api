using System.Text.Json;

namespace FoodplannerModels.Account;

public class UserCredsDTO
{
    public required int Id { get; set; }
    public required string JWT { get; set; }
    public required string Role { get; set; }
    public required bool RoleApproved { get; set; }
}