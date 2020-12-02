using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MBPatients.Models
{
    public class UsersInRole
    {
        public UsersInRole()
        {
            Users = new List<UsersInRole>();
        }
        public string Id { get; set; }
        public string Email { get; set; }

        public string UserName { get; set; }
        public string NormalizedEmail { get; set; }

        public List<UsersInRole> Users { get; set; }
    }
}
