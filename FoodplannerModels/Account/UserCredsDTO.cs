using System.Text.Json;

namespace FoodplannerModels.Account;

public class UserCredsDTO
{
    public required string JWT { get; set; }
    public required UserRole Role { get; set; }
    public required bool RoleApproved { get; set; }
}