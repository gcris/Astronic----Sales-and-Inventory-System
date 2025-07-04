using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class PurchaseOrderReturn
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReferenceNumber { get; set; }

        public virtual ICollection<PurchaseOrderReturnDetail> PurchaseOrderReturnDetailList { get; set; }

        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
