using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account
{
    public class UserArchivedDTO
    {
        [Required]
        public required bool Archived { get; set; }
    }
}
