using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

// Login DTO class for logging in a user
public class LoginDTO
{
    [Required(ErrorMessage = "Email er påkrævet")]
    [EmailAddress(ErrorMessage = "Email er ikke gyldig")]
    [StringLength(100, ErrorMessage = "Email er for lang")] 
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password er påkrævet")]
    [StringLength(100)]
    public required string Password { get; set; }
}