using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public class LoginChild
    {
        [Required(ErrorMessage = "Email er påkrævet")]
        [EmailAddress(ErrorMessage = "Email er ikke gyldig")]
        [StringLength(100, ErrorMessage = "Email er for lang")]
        public required string Email { get; set; }

        [StringLength(6)]
        public string? Code { get; set; }
    }
}
