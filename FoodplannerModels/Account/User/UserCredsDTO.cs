using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FoodplannerModels.Account;

public class UserCredsDTO
{
    [Required]
    public required string JWT { get; set; }
    
    [Required]
    public required string Role { get; set; }
    
    [Required]
    public required bool RoleApproved { get; set; }
}