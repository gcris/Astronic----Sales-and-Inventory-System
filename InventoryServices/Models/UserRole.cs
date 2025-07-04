using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<UserPrivilege> UserPrivilages { get; set; }
    }
}
