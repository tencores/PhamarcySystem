using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AYPatients.Models
{
    public class IndexAndCreateRole
    {
        public IndexAndCreateRole()
        {
            RolesList = new List<IdentityRole>();
        }
               
        public List<IdentityRole> RolesList { get; set; }

        public string RoleName { get; set; } //for creating a role

    }
}
