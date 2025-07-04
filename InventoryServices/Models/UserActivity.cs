using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class UserActivity
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; } // Transaction
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrentStock { get; set; }
        public string Remarks { get; set; }
        public bool Transaction { get; set; }

        public virtual Item Item { get; set; }
        public virtual int? ItemId { get; set; }

        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
