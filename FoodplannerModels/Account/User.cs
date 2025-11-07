namespace FoodplannerModels.Account;
using System.ComponentModel.DataAnnotations;

public class User {
    [Key]
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
    public required bool RoleApproved { get; set; }
    public string PinCode { get; set; }
    public bool Archived { get; set; }
}