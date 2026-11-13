using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

    // Login entity, 
    // entity not currrently used, but LoginDTO is used  
    public class Login
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
