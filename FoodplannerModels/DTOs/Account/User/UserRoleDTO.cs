using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account
{
    // DTO for user role status 
    public class UserRoleDTO
    {
        [Required]
        public bool role_approved { get; set; }
    }
}
