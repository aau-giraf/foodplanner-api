namespace FoodplannerModels.Account;

using System.ComponentModel.DataAnnotations;

[Flags]
public enum UserRole
{
    Admin = 1 << 0,    // 1
    Child = 1 << 1,    // 2
    Teacher = 1 << 2,    // 4
    Parent = 1 << 3     // 8
}
public class User
{
    [Key]
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required UserRole Role { get; set; }
    public required bool RoleApproved { get; set; }
    public string? PinCode { get; set; }
    public bool Archived { get; set; }
}