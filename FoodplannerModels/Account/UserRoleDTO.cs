using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FoodplannerModels.Account
{
    public class UserRoleDTO
    {
        public int id { get; set; }
        public bool role_approved { get; set; }
    }
}
