using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryServices.Models
{
    public class UserPrivilege
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public bool IsEnable { get; set; }

        public virtual int UserRoleId { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}
