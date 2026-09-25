using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account
{
    // Entity class for logging in a child 
    // Used in 'user' controller 
    public class LoginChild
    {
        [Required(ErrorMessage = "Email er påkrævet")]
        [EmailAddress(ErrorMessage = "Email er ikke gyldig")]
        [StringLength(100, ErrorMessage = "Email er for lang")]
        public required string Email { get; set; }

        [StringLength(6)]
        public string? Code { get; set; }
    }
}
