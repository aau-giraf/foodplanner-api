using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account;

public class User {

    [Key]
    public int Id { get; set; }

    public required string First_name { get; set; }
    public required string Last_name { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }
}