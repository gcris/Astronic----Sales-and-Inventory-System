using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public bool IsEnable { get; set; }

        public virtual ICollection<PasswordHistory> PasswordHistories { get; set; }

        public virtual int UserRoleId { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}
