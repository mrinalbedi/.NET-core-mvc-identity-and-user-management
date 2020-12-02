using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MBPatients.Models
{
    public class CreateRole
    {
        [Required]
        public string RoleName { get; set; }


        public CreateRole()
        {
            Users = new List<string>();
        }
        public List<string> Users { get; set; }
    }
}
