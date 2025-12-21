using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public class CreateClassroomDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Klasse navn er for langt")]
        public required string ClassName {get; set;}

    }
}
