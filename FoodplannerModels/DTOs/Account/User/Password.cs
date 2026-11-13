using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account
{
    // Entity for password 
    // Used in user contorller to update password 
    public class Password
    {
        [Required]
        public required string password { get; set; }
    }
}