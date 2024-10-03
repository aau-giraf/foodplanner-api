using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace foodplanner_models.Account
{
    public class UserDTO
    {
        public required string Name { get; set; }

        public required string Email { get; set; }
    }
}
