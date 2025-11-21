using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public class UserDTO
    {
        public int Id { get; set; }
        public required string First_name { get; set; }
        public required string Last_name { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required bool Archived { get; set; }

    }
}

