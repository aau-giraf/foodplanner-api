using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account
{
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "Fornavn er påkrævet")]
        [StringLength(100, ErrorMessage = "Fornavn er for langt")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Efternavn er påkrævet")]
        [StringLength(100, ErrorMessage = "Efternavn er for langt")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email er påkrævet")]
        [EmailAddress(ErrorMessage = "Email er ikke gyldig")]
        [StringLength(100, ErrorMessage = "Email er for langt")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Adgangskode er påkrævet")]
        [StringLength(100, ErrorMessage = "Adgangskode er for langt")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Role er påkrævet")]
        public required string Role { get; set; }
    }
}
