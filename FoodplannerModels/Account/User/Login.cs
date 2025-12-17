using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

    public class Login
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
