using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AYPatients.Models
{
    public class RoleAndUsers
    {
        public RoleAndUsers()
        {
            UsersInThisRole = new List<IdentityUser>(); // already a list here
           
        }

        public string RoleId { get; set; }

        public string UserName { get; set; }

       

        public List<IdentityUser> UsersInThisRole { get; set; } // using the list to show Users in this role, 
                                                           //this integrate the functions of EditUseRole
    }
}
