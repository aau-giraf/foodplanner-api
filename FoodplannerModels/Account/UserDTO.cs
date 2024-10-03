using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace foodplanner_models.Account
{
    public class UserDTO
    {
        public required string First_name { get; set; }
        public required string Last_name { get; set; }

        public required string Email { get; set; }
    }
}
