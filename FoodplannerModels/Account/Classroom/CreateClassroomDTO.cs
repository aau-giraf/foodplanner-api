using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account
{
    public class CreateClassroomDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Klasse navn er for langt")]
        public required string ClassName {get; set;}

    }
}
