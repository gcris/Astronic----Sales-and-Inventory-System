using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryServices.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public DateTime Date { get; set; }

        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
