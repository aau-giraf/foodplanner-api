using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account
{
    public class ChildrenCreateDTO
    {
        [Required(ErrorMessage = "Fornavn er påkrævet")]
        [StringLength(100, ErrorMessage = "Fornavn er for langt")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Efternavn er påkrævet")]
        [StringLength(100, ErrorMessage = "Efternavn er for langt")]
        public required string LastName { get; set; }
        
        [Required(ErrorMessage = "KlasseId er påkrævet")]
        public int classId { get; set; }
        
    }
}
