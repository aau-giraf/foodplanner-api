using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account
{
    public class Password
    {
        [Required]
        public required string password { get; set; }
    }
}